using System;
using System.Collections.Generic;

namespace MorganDI.Builder.ParameterBindings
{
    internal sealed class ServiceParameterBinding : ParameterBinding
    {
        private readonly ServiceIdentifier[] _identifiers;

        public IEnumerable<ServiceIdentifier> Identifiers => _identifiers;

        private ServiceParameterBinding(string name, Type type, ServiceIdentifier[] identifiers) : base(name, type)
        {
            if (!type.IsAssignableFrom(identifiers[0].Type))
                throw new ArgumentException($"Supplied service '{identifiers[0]}' is not compatible with the requested parameter '{name}'.", nameof(identifiers));

            _identifiers = identifiers;
        }

        public override object Resolve(IServiceProvider serviceProvider, Scope scope)
        {
            // Cycle through the defined identifiers, resolve the first identifier
            // that exists in the container.
            foreach(ServiceIdentifier identifier in _identifiers)
            {
                if (serviceProvider.ServiceExists(identifier, scope))
                    return serviceProvider.Resolve(identifier);
            }

            // No service found for the requested identifier, throw an error.
            throw new InvalidDependencyDefinitionException($"No service found that matches the parameter's configuration.\r\n{string.Join("\r\n", _identifiers)}");
        }

        public static ServiceParameterBinding Create(string name, Type type)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // When created from a name and type, create two identifiers,
            // one for a named service matching the parameter name, the other
            // for a default service based on the parameter type
            ServiceIdentifier[] identifiers = new[]
            {
                new ServiceIdentifier(type, name),
                new ServiceIdentifier(type)
            };

            return new ServiceParameterBinding(name, type, identifiers);
        }

        public static ServiceParameterBinding Create(ParameterBinding parameter, ServiceIdentifier identifier)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            return new ServiceParameterBinding(parameter.Name, parameter.ParameterType, new[] { identifier });
        }
    }
}
