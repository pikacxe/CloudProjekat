using System.ServiceModel;

namespace StudentServiceClient.UniversalConnector
{
    /// <summary>
    /// Represents a generic service connector for connecting to a service.
    /// </summary>
    /// <typeparam name="TService">The service interface type.</typeparam>
    public class ServiceConnector<TService> : IServiceConnector<TService>
    {
        private static TService proxy;

        /// <summary>
        /// Connects to the specified service.
        /// </summary>
        /// <param name="serviceAddress">The address of the service.</param>
        public void Connect(string serviceAddress)
        {
            var binding = new NetTcpBinding();
            var endpointAddress = new EndpointAddress(serviceAddress);
            var factory = new ChannelFactory<TService>(binding, endpointAddress);
            proxy = factory.CreateChannel();
        }

        /// <summary>
        /// Gets the proxy instance for the connected service.
        /// </summary>
        /// <returns>The proxy instance for the service.</returns>
        public TService GetProxy()
        {
            return proxy;
        }
    }
}
