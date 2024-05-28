using Common;
using Common.Models;
using Common.Repositories;
using Microsoft.WindowsAzure.ServiceRuntime;
using NotificationService.Helpers;
using StudentServiceClient.UniversalConnector;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private readonly ICloudRepository<HealthCheckInfo> _healthCheckRepository = new CloudRepository<HealthCheckInfo>("HealthCheck");
        private static AdminConsoleService adminConsoleService;
        public override void Run()
        {
            Trace.TraceInformation("HealthMonitoringService is running");

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
            adminConsoleService = new AdminConsoleService();
            adminConsoleService.Open();

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        private async Task TestServicesAsync()
        {
            ServiceConnector<IHealthMonitoringService> serviceConnector = new ServiceConnector<IHealthMonitoringService>();
            
            /// Create new HealthCheckInfo for portfolio and notification service
            Guid portfolioGuid = Guid.NewGuid();
            Guid notificationGuid = Guid.NewGuid();

            HealthCheckInfo portfolioHealthCheck = new HealthCheckInfo(portfolioGuid)
            {
                Id = portfolioGuid
            };

            HealthCheckInfo notificationHealthCheck = new HealthCheckInfo(notificationGuid)
            {
                Id = notificationGuid
            };

            /// Test Portfolio service
            try
            {
                serviceConnector.Connect("net.tcp://localhost:10100/health-monitoring");
                IHealthMonitoringService healthMonitoringService = serviceConnector.GetProxy();
                healthMonitoringService.HealthCheck();

                Trace.WriteLine($"[INFO] {DateTime.UtcNow}_PORTFOLIO_OK");
                portfolioHealthCheck.Message = $"[INFO] {DateTime.UtcNow}_PORTFOLIO_OK";
            }
            catch
            {
                Trace.WriteLine($"[WARNING] {DateTime.UtcNow}_PORTFOLIO_NOT_OK");
                portfolioHealthCheck.Message = $"[WARNING] {DateTime.UtcNow}_PORTFOLIO_NOT_OK";
            }
            /// Test Notification service
            try
            {
                serviceConnector.Connect("net.tcp://localhost:10101/health-monitoring");
                IHealthMonitoringService healthMonitoringService = serviceConnector.GetProxy();
                healthMonitoringService.HealthCheck();

                Trace.WriteLine($"[INFO] {DateTime.UtcNow}_NOTIFICATION_OK");
                notificationHealthCheck.Message = $"[INFO] {DateTime.UtcNow}_NOTIFICATION_OK";
            }
            catch
            {
                Trace.WriteLine($"[WARNING] {DateTime.UtcNow}_NOTIFICATION_NOT_OK");
                notificationHealthCheck.Message = $"[WARNING] {DateTime.UtcNow}_NOTIFICATION_NOT_OK";
            }

            /// Add the messages to the table

            await _healthCheckRepository.Add(portfolioHealthCheck);
            await _healthCheckRepository.Add(notificationHealthCheck);
            if (portfolioHealthCheck.Message.Contains("WARNING"))
            {
                await MailHelper.SendServiceDown(AdminConsoleServiceProvider.adminEmails, "'Portfolio service'");
            }
            if (notificationHealthCheck.Message.Contains("WARNING"))
            {
                await MailHelper.SendServiceDown(AdminConsoleServiceProvider.adminEmails, "'Notification service'");

            }

            /// Send the message to the NotificationService
            /// Maybe the implementation of sending email is better here

            //CloudQueue queue = QueueHelper.GetQueueReference("HealthCheckQueue");
            //queue.AddMessage(new CloudQueueMessage(portfolioHealthCheck.Message));

        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            adminConsoleService.Close();
            base.OnStop();

            Trace.TraceInformation("HealthMonitoringService has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            Random r =  new Random();
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Health service working");
                await TestServicesAsync();
                await Task.Delay(1000 + r.Next(0,4001));
            }
        }
    }
}
