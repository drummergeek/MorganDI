using System;

namespace MorganDI
{
    public static class ServiceFactoryExtensions
    {
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
