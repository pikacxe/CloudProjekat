using HealthMonitoringService;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System;
using System.Collections.Generic;
using System.Linq;

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
