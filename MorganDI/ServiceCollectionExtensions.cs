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
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        public static IServiceCollection AddSingletonService<TService, TInstance>(this IServiceCollection serviceCollection, string name = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Singleton, name);

        /// <summary>
        /// Register a service to a concrete class in the scene scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        public static IServiceCollection AddSceneService<TService, TInstance>(this IServiceCollection serviceCollection, string name = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Scene, name);

        /// <summary>
        /// Register a service to a concrete class in the transient scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        public static IServiceCollection AddTransientService<TService, TInstance>(this IServiceCollection serviceCollection, string name = null)
            where TService : class
            where TInstance : TService =>
                AddServiceToScope<TService, TInstance>(serviceCollection, Scope.Transient, name);

        /// <summary>
        /// Register a service to a concrete class in the supplied scope of the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <typeparam name="TInstance">The concrete type used to resolve the service.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which to add the registration.</param>
        /// <param name="scope">The <see cref="Scope"/> the service in which the service is registered.</param>
        /// <param name="name">The optional name for the service, if null or empty it will be the default provider.</param>
        public static IServiceCollection AddServiceToScope<TService, TInstance>(this IServiceCollection serviceCollection, Scope scope, string name = null)
            where TService : class
            where TInstance : TService
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddService(typeof(TService), name, scope, typeof(TInstance));

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

            serviceCollection.AddServiceDelegate<TService>(name, scope, serviceDelegate);

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

            serviceCollection.AddServiceInstance(typeof(TService), name, value);

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

            serviceCollection.AddServiceAlias(typeof(TAlias), aliasName, typeof(TService), serviceName);

            return serviceCollection;
        }

        /// <summary>
        /// Bind a parameter on the last service registration to a specific service registered in the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The service to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which the assoicated service was last added.</param>
        /// <param name="parameterName">The name of the parameter to be configured.</param>
        /// <param name="serviceName">The optional name of the service to be bound.</param>
        public static IServiceCollection BindParameterToService<TService>(this IServiceCollection serviceCollection, string parameterName, string serviceName = null)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.BindParameter(parameterName, typeof(TService), serviceName);

            return serviceCollection;
        }

        /// <summary>
        /// Bind a parameter on the last service registration to a static value in the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which the assoicated service was last added.</param>
        /// <param name="parameterName">The name of the parameter to be configured.</param>
        /// <param name="value">The static value to bind to the parameter.</param>
        public static IServiceCollection BindParameterToValue(this IServiceCollection serviceCollection, string parameterName, object value)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.BindParameter(parameterName, value);

            return serviceCollection;
        }

        /// <summary>
        /// Bind a parameter on the last service registration to a delegate in the provided <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The return type of the delegate to be bound.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> in which the assoicated service was last added.</param>
        /// <param name="parameterName">The name of the parameter to be configured.</param>
        /// <param name="serviceDelegate">The delegate to be invoked to resolve the parameter.</param>
        public static IServiceCollection BindParameterToDelegate<TService>(this IServiceCollection serviceCollection, string parameterName, ServiceDelegate<TService> serviceDelegate)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.BindParameter(parameterName, serviceDelegate);

            return serviceCollection;
        }
    }
}
