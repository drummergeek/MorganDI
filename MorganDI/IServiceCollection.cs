using System;
using System.Collections.Generic;

namespace MorganDI
{
    public delegate TService ServiceDelegate<TService>(IServiceProvider serviceProvider);

    public interface IServiceCollection : IEnumerable<IServiceResolver>
    {
        IServiceResolver LastAddedService { get; }

        bool ContainsService(ServiceIdentifier identifier);
        void AddService(IServiceResolver serviceResolver);

        void AddService(Type serviceType, string name, Scope scope, Type instanceType);
        void AddServiceDelegate<TService>(string name, Scope scope, ServiceDelegate<TService> serviceDelegate);
        void AddServiceInstance(Type serviceType, string name, object value);
        void AddServiceAlias(Type aliasType, string aliasName, Type serviceType, string serviceName);

        void BindParameter(string parameterName, Type serviceType, string serviceName);
        void BindParameter<TService>(string parameterName, ServiceDelegate<TService> serviceDelegate);
        void BindParameter(string parameterName, object value);
    }
}
