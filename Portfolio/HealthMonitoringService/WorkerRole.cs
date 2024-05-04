using Common;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using StudentServiceClient.UniversalConnector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

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

            Trace.TraceInformation("HealthMonitoringService has been started");

            return result;
        }

        private void TestServices()
        {
            /// Test Portfolio service
            ServiceConnector<IHealthMonitoringService> serviceConnector = new ServiceConnector<IHealthMonitoringService>();
            try
            {
                serviceConnector.Connect("net.tcp://localhost:10100/health-monitoring");
                IHealthMonitoringService healthMonitoringService = serviceConnector.GetProxy();
                healthMonitoringService.HealthCheck();
                Trace.WriteLine($"[INFO] {DateTime.UtcNow}-PORTFOLIO_OK");
            }
            catch
            {
                Trace.WriteLine($"[WARNING] {DateTime.UtcNow}-PORTFOLIO_NOT_OK");
            }
            /// Test Notification service
            try
            {
                serviceConnector.Connect("net.tcp://localhost:10101/health-monitoring");
                IHealthMonitoringService healthMonitoringService = serviceConnector.GetProxy();
                healthMonitoringService.HealthCheck();
                Trace.WriteLine($"[INFO] {DateTime.UtcNow}-NOTIFICATION_OK");
            }
            catch
            {
                Trace.WriteLine($"[WARNING] {DateTime.UtcNow}-NOTIFICATION_NOT_OK");
            }
        }

        public override void OnStop()
        {
            Trace.TraceInformation("HealthMonitoringService is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

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
                TestServices();
                await Task.Delay(1000 + r.Next(0,4001));
            }
        }
    }
}
