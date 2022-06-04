using System;
using System.Collections.Generic;

namespace MorganDI
{
    public delegate TService ServiceDelegate<TService>(IServiceProvider serviceProvider);

    /// <summary>
    /// Represents a collection of configured service registrations.
    /// </summary>
    public interface IServiceCollection : IEnumerable<ServiceRegistration>
    {
        /// <summary>
        /// Returns whether or not the requested service is registered in the collection at or below the requested scope.
        /// </summary>
        /// <param name="identifier">The identifer of the requested service.</param>
        /// <param name="scope">The requested scope.</param>
        bool Contains(ServiceIdentifier identifier, Scope scope);

        /// <summary>
        /// Returns the current <see cref="ServiceRegistration"/> for the requested service.
        /// </summary>
        /// <param name="identifier">The identifier of the requested service.</param>
        ServiceRegistration Get(ServiceIdentifier identifier);

        /// <summary>
        /// Adds the provided service to the collection.
        /// </summary>
        /// <param name="serviceRegistration">The service to be added to the collection.</param>
        void Add(ServiceRegistration serviceRegistration);
    }
}
