using System;

namespace MorganDI
{
    public sealed class AliasServiceRegistration : ServiceRegistration
    {
        /// <summary>
        /// The identifier of the service to be returned when this service is requested.
        /// </summary>
        public ServiceIdentifier Service { get; }

        internal AliasServiceRegistration(ServiceIdentifier identifier, ServiceIdentifier service)
            : base(identifier, Scope.Singleton)
        {
            if (identifier == service)
                throw new ArgumentException($"Service '{identifier}' cannot alias to itself!", nameof(service));
            if (!identifier.Type.IsAssignableFrom(service.Type))
                throw new ArgumentException($"Service '{service}' is not assignable to the requested service '{identifier}'.", nameof(service));

            Service = service;
        }
    }
}
