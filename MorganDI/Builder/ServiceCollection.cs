using System;
using System.Collections;
using System.Collections.Generic;

namespace MorganDI.Builder
{
    /// <inheritdoc cref="IServiceCollection"/>
    public sealed class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<ServiceIdentifier, ServiceRegistration> _services =
            new Dictionary<ServiceIdentifier, ServiceRegistration>();

        internal ServiceCollection() { }

        /// <inheritdoc cref="IServiceCollection.Contains(ServiceIdentifier)"/>
        public bool Contains(ServiceIdentifier identifier, Scope scope)
        {
            if (!_services.ContainsKey(identifier))
                return false;

            var service = _services[identifier];

            // If the registration is an alias, ensure it exists and check the scope of the root service.
            if (service is AliasServiceRegistration aliasService)
            {
                if (!_services.ContainsKey(aliasService.Service))
                    return false;

                service = _services[aliasService.Service];
            }

            return service.Scope <= scope;
        }

        /// <inheritdoc cref="IServiceCollection.Get(ServiceIdentifier)"/>
        public ServiceRegistration Get(ServiceIdentifier identifier) => _services[identifier];

        /// <inheritdoc cref="IServiceCollection.Add(ServiceRegistration)"/>
        public void Add(ServiceRegistration service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (_services.ContainsKey(service.Identifier))
                throw new ArgumentException($"Service '{service.Identifier}' already registered!", nameof(service));

            _services.Add(service.Identifier, service);
        }

        /// <inheritdoc cref="IEnumerable{ServiceRegistration}.GetEnumerator"/>
        public IEnumerator<ServiceRegistration> GetEnumerator() => _services.Values.GetEnumerator();

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
