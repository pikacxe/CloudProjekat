using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IHealthMonitoringService
    {
        [OperationContract]
        void HealthCheck();
    }
}
