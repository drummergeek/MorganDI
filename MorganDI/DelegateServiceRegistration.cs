using System;

namespace MorganDI
{
    /// <summary>
    /// Provides a delegate for a service registered in an <see cref="IServiceCollection"/>.
    /// </summary>
    public abstract class DelegateServiceRegistration : ServiceRegistration
    {
        protected DelegateServiceRegistration(ServiceIdentifier identifier, Scope scope)
            : base(identifier, scope)
        { }

        /// <summary>
        /// Invoke the service delegate and return the instance.
        /// </summary>
        /// <param name="serviceProvider">The current service provider to use during invocation.</param>
        /// <returns></returns>
        public abstract object Resolve(IServiceProvider serviceProvider);
    }

    /// <inheritdoc cref="DelegateServiceRegistration"/>
    public sealed class DelegateServiceRegistration<TService> : DelegateServiceRegistration
    {
        /// <summary>
        /// Returns the delegate used to instantiate the configured service.
        /// </summary>
        public ServiceDelegate<TService> ServiceDelegate { get; }

        internal DelegateServiceRegistration(ServiceIdentifier identifier, Scope scope, ServiceDelegate<TService> serviceDelegate)
            : base(identifier, scope)
        {
            if (serviceDelegate == null)
                throw new ArgumentNullException(nameof(serviceDelegate));

            if (!identifier.Type.IsAssignableFrom(typeof(TService)))
                throw new ArgumentException($"Delegate return type is not compatible with the supplied service, {identifier}", nameof(serviceDelegate));

            ServiceDelegate = serviceDelegate;
        }

        /// <inheritdoc cref="DelegateServiceRegistration.Resolve(IServiceProvider)"/>
        public override object Resolve(IServiceProvider serviceProvider) => ServiceDelegate(serviceProvider);
    }
}
