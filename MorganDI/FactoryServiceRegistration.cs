using System;

namespace MorganDI
{
    /// <summary>
    /// Provides information for a service created by a factory for a service registered in an <see cref="IServiceCollection"/>.
    /// </summary>
    public sealed class FactoryServiceRegistration : ServiceRegistration
    {
        /// <summary>
        /// Returns the type of the service to be instantiated for the configured service.
        /// </summary>
        public Type ServiceType { get; }

        /// <summary>
        /// Returns the configuration delegate for the factory to use to instantiate the configured service.
        /// </summary>
        public ServiceFactoryConfigurationHandler FactoryConfigurationHandler { get; }

        internal FactoryServiceRegistration(ServiceIdentifier identifier, Scope scope, Type serviceType, ServiceFactoryConfigurationHandler factoryConfigurationHandler)
            : base(identifier, scope)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            if (!identifier.Type.IsAssignableFrom(serviceType))
                throw new ArgumentException($"Provided service type is not compatible with the supplied service, {identifier}", nameof(serviceType));

            ServiceType = serviceType;
            FactoryConfigurationHandler = factoryConfigurationHandler;
        }
    }
}
