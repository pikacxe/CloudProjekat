using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoringService
{
    internal class HealthMonitoringServiceProvider : IHealthMonitoringService
    {
        public void HealthCheck()
        {
            Trace.WriteLine("Alive");
        }
    }
}
