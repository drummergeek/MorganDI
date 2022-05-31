namespace MorganDI
{
    public static class ServiceProviderExtensions
    {        
        /// <summary>
        /// Return an instance of the requested service.
        /// </summary>
        /// <typeparam name="TService">The type of the service requested.</typeparam>
        /// <param name="container">The service provider containing the service.</param>
        /// <param name="name">The optional instance name of the service requested.</param>
        public static TService Resolve<TService>(this IServiceProvider container, string name = null)
        {
            return (TService)container.Resolve(ServiceIdentifier.Create<TService>(name));
        }
    }
}
