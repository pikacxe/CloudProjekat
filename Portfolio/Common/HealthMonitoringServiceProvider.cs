using Common;
using System.Diagnostics;

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
