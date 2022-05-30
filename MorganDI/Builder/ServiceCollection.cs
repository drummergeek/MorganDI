using MorganDI.BindingParameters;
using MorganDI.Resolvers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MorganDI
{
    internal class ServiceCollection : IServiceCollection
    {
        protected HashSet<ServiceIdentifier> DefinedServices { get; } = new HashSet<ServiceIdentifier>();
        protected HashSet<IServiceResolver> Services { get; } = new HashSet<IServiceResolver>();

        public virtual IServiceResolver LastAddedService { get; private set; }

        public virtual void AddService(IServiceResolver serviceResolver)
        {
            LastAddedService = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
            DefinedServices.Add(serviceResolver.Identifier);
            Services.Add(serviceResolver);
        }

        public virtual void AddService(Type serviceType, string name, Scope scope, Type instanceType) =>
            AddService(new ConstructorServiceResolver(new ServiceIdentifier(serviceType, name), scope, instanceType));

        public virtual void AddServiceAlias(Type aliasType, string aliasName, Type serviceType, string serviceName)
        {
            ServiceIdentifier aliasIdentifier = new ServiceIdentifier(aliasType, aliasName);

            if (ContainsService(aliasIdentifier))
                throw new InvalidDependencyDefinitionException($"Alias service '{aliasIdentifier}' is already defined.");

            ServiceIdentifier serviceIdentifier = new ServiceIdentifier(serviceType, serviceName);

            if (!ContainsService(serviceIdentifier))
                throw new InvalidDependencyDefinitionException($"Service '{serviceIdentifier}' not found.");

            IServiceResolver resolver = new AliasServiceResolver(aliasIdentifier, serviceIdentifier);

            AddService(resolver);
        }

        public virtual void AddServiceDelegate<TService>(string name, Scope scope, ServiceDelegate<TService> serviceDelegate) =>
            AddService(new DelegateServiceResolver<TService>(ServiceIdentifier.Create<TService>(name), scope, serviceDelegate));

        public virtual void AddServiceInstance(Type serviceType, string name, object value) =>
            AddService(new StaticServiceResolver(new ServiceIdentifier(serviceType, name), value));

        public virtual void BindParameter(string parameterName, Type serviceType, string serviceName) =>
            SetBindingParameter(parameterName, p => ServiceBindingParameter.Create(p, new ServiceIdentifier(serviceType, serviceName)));

        public virtual void BindParameter<TService>(string parameterName, ServiceDelegate<TService> serviceDelegate) =>
            SetBindingParameter(parameterName, p => DelegateBindingParameter<TService>.Create(p, serviceDelegate));

        public virtual void BindParameter(string parameterName, object value) =>
            SetBindingParameter(parameterName, p => StaticBindingParameter.Create(p, value));
        
        private void SetBindingParameter(string parameterName, Func<IBindingParameter, IBindingParameter> bindingParameterConstructionCallback)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException(nameof(parameterName));

            if (LastAddedService == null)
                throw new InvalidDependencyDefinitionException("No services have been registered on which to set binding parameters.");

            if (!(LastAddedService is IParameterizedServiceResolver resolver))
                throw new InvalidDependencyDefinitionException("Last registered service does not support binding parameters.");

            var parameter = resolver.GetBindingParameter(parameterName);

            if (parameter == null)
                throw new InvalidDependencyDefinitionException("No parameter was found matching the supplied parameterName");

            var newParameter = bindingParameterConstructionCallback.Invoke(parameter);

            resolver.SetBindingParameter(newParameter);
        }

        public virtual bool ContainsService(ServiceIdentifier identifier) => DefinedServices.Contains(identifier);

        public virtual IEnumerator<IServiceResolver> GetEnumerator() => Services.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
