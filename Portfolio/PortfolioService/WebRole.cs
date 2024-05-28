using HealthMonitoringService;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Diagnostics;
using System.Threading;
using System;
using Common.Helpers;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace PortfolioService
{
    public class WebRole : RoleEntryPoint
    {
        private static HealthMonitoringServer healthMonitoringServer;

        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.
            bool ret = base.OnStart();
            healthMonitoringServer = new HealthMonitoringServer();
            healthMonitoringServer.Open();
            return ret;
        }

        public override void Run()
        {
            try
            {
                
                CloudQueue queue = QueueHelper.GetQueueReference("alarmsqueue");
                while (true)
                {
                    CloudQueueMessage message = queue.GetMessage();
                    if (message == null)
                    {
                        Trace.TraceInformation("No messages in queue.", "Information");
                    }
                    else
                    {
                        Trace.TraceInformation($"Queue message: {message.AsString}");

                        queue.DeleteMessage(message);
                    }

                    Thread.Sleep(5000);
                    Trace.TraceInformation("Working", "Information");
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        public override void OnStop()
        {
            healthMonitoringServer.Close();
            base.OnStop();
        }
    }
}
