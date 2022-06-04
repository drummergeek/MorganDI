using System;

namespace MorganDI
{
    /// <summary>
    /// Provides a static instance for a service registered in an <see cref="IServiceCollection"/>.
    /// </summary>
    public sealed class StaticServiceRegistration : ServiceRegistration
    {
        /// <summary>
        /// Returns the static instance associated with the configured service.
        /// </summary>
        public object Instance { get; }

        internal StaticServiceRegistration(ServiceIdentifier identifier, object instance)
            : base(identifier, Scope.Singleton)
        {
            if (instance != null && !identifier.Type.IsAssignableFrom(instance.GetType()))
                throw new ArgumentException($"The type of the supplied value '{instance.GetType()}' is not assignable to the supplied service '{identifier}'.", nameof(instance));

            Instance = instance;
        }
    }
}
