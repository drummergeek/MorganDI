using System;

namespace MorganDI
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register a service to a concrete class in the singleton scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
        public static IServiceCollection AddSingletonService<TService, TInstance>(this IServiceCollection serviceCollection, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Singleton, factoryConfigurationHandler);

        /// <summary>
        /// Register a service to a concrete class in the singleton scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
        public static IServiceCollection AddSingletonService<TService, TInstance>(this IServiceCollection serviceCollection, string name, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Singleton, name, factoryConfigurationHandler);

        /// <summary>
        /// Register a service to a concrete class in the scene scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
        public static IServiceCollection AddSceneService<TService, TInstance>(this IServiceCollection serviceCollection, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Scene, factoryConfigurationHandler);

        /// <summary>
        /// Register a service to a concrete class in the scene scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
        public static IServiceCollection AddSceneService<TService, TInstance>(this IServiceCollection serviceCollection, string name, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Scene, name, factoryConfigurationHandler);

        /// <summary>
        /// Register a service to a concrete class in the transient scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
        public static IServiceCollection AddTransientService<TService, TInstance>(this IServiceCollection serviceCollection, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Transient, factoryConfigurationHandler);

        /// <summary>
        /// Register a service to a concrete class in the transient scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
        public static IServiceCollection AddTransientService<TService, TInstance>(this IServiceCollection serviceCollection, string name, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Transient, name, factoryConfigurationHandler);

        /// <summary>
        /// Register a service to a concrete class in the supplied scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="scope">The <see cref="Scope"/> the service in which the service is registered.</param>
        /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
        public static IServiceCollection AddServiceToScope<TService, TInstance>(this IServiceCollection serviceCollection, Scope scope, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, scope, null, factoryConfigurationHandler);

            /// <summary>
            /// Register a service to a concrete class in the supplied scope of the provided <see cref="IServiceCollection"/>.
            /// </summary>
            /// <typeparam name="TService">The service to be bound.</typeparam>
            /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
            /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
            /// <param name="scope">The <see cref="Scope"/> the service in which the service is registered.</param>
            /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
            /// <param name="factoryConfigurationHandler">Optional delegate to configure the service factory for instantiating the service.</param>
            public static IServiceCollection AddServiceToScope<TService, TInstance>(this IServiceCollection serviceCollection, Scope scope, string name, ServiceFactoryConfigurationHandler factoryConfigurationHandler = null)
            where TService : class
            where TInstance : TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            ServiceIdentifier identifier = ServiceIdentifier.Create<TService>(name);
            FactoryServiceRegistration registration = new FactoryServiceRegistration(identifier, scope, typeof(TInstance), factoryConfigurationHandler);

            serviceCollection.Add(registration);
            
            return serviceCollection;
        }

        /// <summary>
        /// Register a service to a delegate in the singleton scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddSingletonService<TService>(this IServiceCollection serviceCollection, ServiceDelegate<TService> serviceDelegate)
            where TService : class =>
                AddSingletonService(serviceCollection, null, serviceDelegate);

        /// <summary>
        /// Register a service to a delegate in the singleton scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddSingletonService<TService>(this IServiceCollection serviceCollection, string name, ServiceDelegate<TService> serviceDelegate)
            where TService : class =>
                AddServiceToScope(serviceCollection, Scope.Singleton, name, serviceDelegate);

        /// <summary>
        /// Register a service to a delegate in the scene scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddSceneService<TService>(this IServiceCollection serviceCollection, ServiceDelegate<TService> serviceDelegate)
            where TService : class =>
                AddSceneService(serviceCollection, null, serviceDelegate);

        /// <summary>
        /// Register a service to a delegate in the scene scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddSceneService<TService>(this IServiceCollection serviceCollection, string name, ServiceDelegate<TService> serviceDelegate)
            where TService : class =>
                AddServiceToScope(serviceCollection, Scope.Scene, name, serviceDelegate);

        /// <summary>
        /// Register a service to a delegate in the transient scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddTransientService<TService>(this IServiceCollection serviceCollection, ServiceDelegate<TService> serviceDelegate)
            where TService : class =>
                AddTransientService(serviceCollection, null, serviceDelegate);

        /// <summary>
        /// Register a service to a delegate in the transient scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddTransientService<TService>(this IServiceCollection serviceCollection, string name, ServiceDelegate<TService> serviceDelegate)
            where TService : class =>
                AddServiceToScope(serviceCollection, Scope.Transient, name, serviceDelegate);

        /// <summary>
        /// Register a service to a delegate in the requested scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="scope">The <see cref="Scope"/> the service in which the service is registered.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddServiceToScope<TService>(this IServiceCollection serviceCollection, Scope scope, ServiceDelegate<TService> serviceDelegate)
            where TService : class =>
                AddServiceToScope(serviceCollection, scope, null, serviceDelegate);

        /// <summary>
        /// Register a service to a delegate in the requested scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="scope">The <see cref="Scope"/> the service in which the service is registered.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the service.</param>
        public static IServiceCollection AddServiceToScope<TService>(this IServiceCollection serviceCollection, Scope scope, string name, ServiceDelegate<TService> serviceDelegate)
            where TService : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            ServiceIdentifier identifier = ServiceIdentifier.Create<TService>(name);
            DelegateServiceRegistration<TService> registration = new DelegateServiceRegistration<TService>(identifier, scope, serviceDelegate);

            serviceCollection.Add(registration);

            return serviceCollection;
        }

        /// <summary>
        /// Register a service to a static value in the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="value">The static value to associate with the service.</param>
        public static IServiceCollection AddStaticInstance<TService>(this IServiceCollection serviceCollection, TService value)
            where TService : class =>
                AddStaticInstance(serviceCollection, null, value);

        /// <summary>
        /// Register a service to a static value in the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="value">The static value to associate with the service.</param>
        public static IServiceCollection AddStaticInstance<TService>(this IServiceCollection serviceCollection, string name, TService value)
            where TService : class
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            ServiceIdentifier identifier = ServiceIdentifier.Create<TService>(name);
            StaticServiceRegistration registration = new StaticServiceRegistration(identifier, value);

            serviceCollection.Add(registration);

            return serviceCollection;
        }

        /// <summary>
        /// Register a service that is an alias to an existing service the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TAlias">The service to be bound.</typeparam>
        /// <typeparam name="TService">The service on which the alias it to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="aliasName">The optional name for the service, if null or empty it will be the default provider.</param>
        /// <param name="serviceName">The optional name of the existing service.</param>
        public static IServiceCollection AddServiceAlias<TAlias, TService>(this IServiceCollection serviceCollection, string aliasName = null, string serviceName = null)
            where TAlias : class
            where TService : TAlias
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            ServiceIdentifier alias = ServiceIdentifier.Create<TAlias>(aliasName);
            ServiceIdentifier service = ServiceIdentifier.Create<TService>(serviceName);

            AliasServiceRegistration registration = new AliasServiceRegistration(alias, service);

            serviceCollection.Add(registration);

            return serviceCollection;
        }

        /// <summary>
        /// Bind a construction parameter on the service factory to an existing service.
        /// </summary>
        /// <typeparam name="TService">The type of the service to be bound.</typeparam>
        /// <param name="serviceFactory">The <see cref="IServiceFactory"/> handling the instantiation.</param>
        /// <param name="parameterName">The name of the parameter to be configured.</param>
        /// <param name="serviceName">The optional name of the service to be bound.</param>
        public static IServiceFactory BindParameterToService<TService>(this IServiceFactory serviceFactory, string parameterName, string serviceName = null)
        {
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            serviceFactory.BindParameter(parameterName, ServiceIdentifier.Create<TService>(serviceName));

            return serviceFactory;
        }

        /// <summary>
        /// Bind a construction parameter on the service factory to a static value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to be bound.</typeparam>
        /// <param name="serviceFactory">The <see cref="IServiceFactory"/> handling the instantiation.</param>
        /// <param name="parameterName">The name of the parameter to be configured.</param>
        /// <param name="value">The static value to bind to the parameter.</param>
        public static IServiceFactory BindParameterToValue<TValue>(this IServiceFactory serviceFactory, string parameterName, TValue value)
        {
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            serviceFactory.BindParameter(parameterName, value);

            return serviceFactory;
        }

        /// <summary>
        /// Bind a construction parameter on the service factory to a delegate.
        /// </summary>
        /// <typeparam name="TService">The return type of the delegate to be bound.</typeparam>
        /// <param name="serviceFactory">The <see cref="IServiceFactory"/> handling the instantiation.</param>
        /// <param name="parameterName">The name of the parameter to be configured.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the parameter.</param>
        public static IServiceFactory BindParameterToDelegate<TService>(this IServiceFactory serviceFactory, string parameterName, ServiceDelegate<TService> serviceDelegate)
        {
            if (serviceFactory == null)
                throw new ArgumentNullException(nameof(serviceFactory));

            serviceFactory.BindParameter(parameterName, serviceDelegate);

            return serviceFactory;
        }
    }
}
