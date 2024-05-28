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
using PortfolioService.Controllers;

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

        public override void OnStop()
        {
            healthMonitoringServer.Close();
            base.OnStop();
        }
    }
}
