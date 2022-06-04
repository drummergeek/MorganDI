using MorganDI.Builder.ParameterBindings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MorganDI.Builder
{
    internal class ReflectionServiceFactory : IServiceFactory
    {
        private class ParameterSet : IEnumerable<ParameterBinding>
        {
            private readonly Dictionary<string, int> _parameterIndexes = new Dictionary<string, int>();
            private readonly ParameterBinding[] _parameters;

            public int Count => _parameters.Length;

            public ParameterBinding this[string name]
            {
                get => this[_parameterIndexes[name]];
                set => this[_parameterIndexes[name]] = value;
            }

            public ParameterBinding this[int index]
            {
                get => _parameters[index];
                set => _parameters[index] = value;
            }

            public ParameterSet(ParameterInfo[] parameters)
            {
                _parameters = new ParameterBinding[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    ParameterInfo parameter = parameters[i];
                    _parameterIndexes.Add(parameter.Name, i);

                    ServiceIdentifier[] serviceIdentifiers = new[]
                    {
                        new ServiceIdentifier(parameter.ParameterType, parameter.Name),
                        new ServiceIdentifier(parameter.ParameterType),
                    };

                    _parameters[i] = ServiceParameterBinding.Create(parameter.Name, parameter.ParameterType);
                }
            }

            public bool ParameterExists(string name) => _parameterIndexes.ContainsKey(name);

            public object[] ResolveAll(IServiceProvider serviceProvider, Scope scope)
            {
                object[] parameters = new object[_parameters.Length];

                for (int i = 0; i < _parameters.Length; i++)
                {
                    parameters[i] = _parameters[i].Resolve(serviceProvider, scope);
                }

                return parameters;
            }

            public IEnumerator<ParameterBinding> GetEnumerator() => ((IEnumerable<ParameterBinding>)_parameters).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private readonly Type _serviceType;
        private readonly ConstructorInfo _constructorInfo;
        private readonly ParameterSet _parameters;

        public ReflectionServiceFactory(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            ConstructorInfo[] validConstructors = serviceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (validConstructors.Length == 0)
                throw new ArgumentException($"Unable to instantiate requested type, '{serviceType.FullName}'.", nameof(serviceType));

            _serviceType = serviceType;
            _constructorInfo = validConstructors[0];
            _parameters = new ParameterSet(_constructorInfo.GetParameters());
        }

        public HashSet<ServiceIdentifier> GetRequiredServices(IServiceProvider serviceProvider, Scope scope)
        {
            HashSet<ServiceIdentifier> requiredServices = new HashSet<ServiceIdentifier>();

            foreach(ParameterBinding parameter in _parameters)
            {
                if (!(parameter is ServiceParameterBinding serviceParameter))
                    continue;

                bool serviceFound = false;
                foreach (ServiceIdentifier identifier in serviceParameter.Identifiers)
                {
                    if (!serviceProvider.ServiceExists(identifier, scope))
                        continue;

                    serviceFound = true;
                    requiredServices.Add(identifier);
                    break;
                }

                if (!serviceFound)
                    throw new InvalidDependencyDefinitionException($"No service found for parameter '{parameter.Name}' on constructor for '{_serviceType.FullName}");
            }

            return requiredServices;
        }

        public object Instantiate(IServiceProvider serviceProvider, Scope scope) =>
            _constructorInfo.Invoke(_parameters.ResolveAll(serviceProvider, scope));

        private ParameterBinding GetParameter(string parameterName) 
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException(nameof(parameterName));

            if (!_parameters.ParameterExists(parameterName))
                throw new ArgumentException($"No matching parameter with name '{parameterName}' found.", nameof(parameterName));

            return _parameters[parameterName];
        }

        public void BindParameter(string parameterName, ServiceIdentifier serviceIdentifier)
        {
            ParameterBinding currentParameter = GetParameter(parameterName);
            _parameters[parameterName] = ServiceParameterBinding.Create(currentParameter, serviceIdentifier);
        }

        public void BindParameter<TService>(string parameterName, ServiceDelegate<TService> serviceDelegate)
        {
            ParameterBinding currentParameter = GetParameter(parameterName);
            _parameters[parameterName] = DelegateParameterBinding<TService>.Create(currentParameter, serviceDelegate);
        }

        public void BindParameter(string parameterName, object value)
        {
            ParameterBinding currentParameter = GetParameter(parameterName);
            _parameters[parameterName] = StaticBindingParameter.Create(currentParameter, value);
        }
    }
}
