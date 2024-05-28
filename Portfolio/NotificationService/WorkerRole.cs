using Common.Models;
using Common.Repositories;
using Common.Helpers;
using HealthMonitoringService;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Queue;
using NotificationService.Helpers;
using System.Collections.Generic;
using System;

namespace NotificationService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly ICloudRepository<ProfitAlarm> _activeAlarmRepo = new CloudRepository<ProfitAlarm>("ActiveAlarmsTable");
        private readonly ICloudRepository<ProfitAlarm> _doneAlarmRepo = new CloudRepository<ProfitAlarm>("DoneAlarmsTable");


        private static HealthMonitoringServer healthMonitoringServer;
        public override void Run()
        {
            Trace.TraceInformation("NotificationService is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Use TLS 1.2 for Service Bus connections
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();
            healthMonitoringServer = new HealthMonitoringServer();
            healthMonitoringServer.Open();

            Trace.TraceInformation("NotificationService has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("NotificationService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            healthMonitoringServer.Close();
            base.OnStop();

            Trace.TraceInformation("NotificationService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    Trace.TraceInformation("[NOTIFICATION_SERVICE] Alarm processing started....");
                    await ProcessAlarmsAsync();
                    Trace.TraceInformation("[NOTIFICATION_SERVICE] Alarm processing completed....");
                    await Task.Delay(10000);
                }
                catch (Exception)
                {
                    Trace.TraceError("Error while proccessing alarms");
                }
            }
        }

        private async Task ProcessAlarmsAsync()
        {
            // Get at most 20 alarms from table
            var alarmsToProcess = await _activeAlarmRepo.GetAll();
            Trace.WriteLine(alarmsToProcess.Count());
            List<Guid> doneAlarmIds = new List<Guid>();
            ProfitAlarm testAlarm = new ProfitAlarm("test@test.com");
            testAlarm.ProfitMargin = 1000;
            testAlarm.CryptoCurrencyName = "BTC";
            testAlarm.DateCreated = DateTime.Now;
            alarmsToProcess.Append(testAlarm);
            // Check profit for each of them
            foreach (var alarm in alarmsToProcess)
            {
                var res = await CheckProfit(alarm);
                if (res)
                {
                    //await MailHelper.SendAlarmTriggered(alarm);
                    // Perisist done alarm info
                    await _doneAlarmRepo.Add(alarm);
                    // Add done alarm id to list
                    doneAlarmIds.Add(alarm.ProfitAlarmId);
                    // Remove completed alarm froma active alarm table
                    await _activeAlarmRepo.Delete(alarm.RowKey);
                }
            }
            // Enqueue done alarmIds
            EnqueueAlarmIds(doneAlarmIds);
        }

        private void EnqueueAlarmIds(List<Guid> doneAlarmIds)
        {
            CloudQueue _alarmsQueue = QueueHelper.GetQueueReference("AlarmsQueue");
            string message = "";
            foreach (var alarmId in doneAlarmIds)
            {
                message += $"{alarmId}|";
            }
            _alarmsQueue.AddMessageAsync(new CloudQueueMessage(message));
        }

        private async Task<bool> CheckProfit(ProfitAlarm alarm)
        {
            var name = alarm.CryptoCurrencyName;
            // Get currency price from external API
            var price = await CryptoInformationHelper.CheckPrice(name);
            Trace.WriteLine($"Price:{price}");
            return price >= alarm.ProfitMargin;

        }
    }
}
