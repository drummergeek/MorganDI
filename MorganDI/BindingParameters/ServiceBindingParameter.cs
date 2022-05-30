using System;
using System.Collections.Generic;
using System.Reflection;

namespace MorganDI.BindingParameters
{
    internal class ServiceBindingParameter : BindingParameter, IServiceBindingParameter
    {
        private readonly ServiceIdentifier[] _identifiers;

        public virtual IEnumerable<ServiceIdentifier> Identifiers => _identifiers;

        private ServiceBindingParameter(string name, Type type, ServiceIdentifier[] identifiers) : base(name, type)
        {
            if (!type.IsAssignableFrom(identifiers[0].Type))
                throw new ArgumentException($"Supplied service '{identifiers[0]}' is not compatible with the requested parameter '{name}'.", nameof(identifiers));

            _identifiers = identifiers;
        }

        public override object Resolve(IServiceProvider container, Scope scope)
        {
            // Cycle through the defined identifiers, resolve the first identifier
            // that exists in the container.
            foreach(ServiceIdentifier identifier in _identifiers)
            {
                if (container.ServiceExists(identifier, scope))
                    return container.Resolve(identifier);
            }

            // No service found for the requested identifier, throw an error.
            throw new InvalidDependencyDefinitionException($"No service found that matches the parameter's configuration.\r\n{string.Join("\r\n", _identifiers)}");
        }

        public static ServiceBindingParameter Create(ParameterInfo parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            // When created from a ParameterInfo object, create two identifiers,
            // one for a named service matching the parameter name, the other
            // for a default service based on the parameter type
            ServiceIdentifier[] identifiers = new[]
            {
                new ServiceIdentifier(parameter.ParameterType, parameter.Name),
                new ServiceIdentifier(parameter.ParameterType)
            };

            return new ServiceBindingParameter(parameter.Name, parameter.ParameterType, identifiers);
        }

        public static ServiceBindingParameter Create(IBindingParameter parameter, ServiceIdentifier identifier)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            return new ServiceBindingParameter(parameter.Name, parameter.ParameterType, new[] { identifier });
        }
    }
}
