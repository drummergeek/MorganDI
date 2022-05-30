namespace MorganDI
{
    /// <summary>
    /// Provides the ability to resolve a <see cref="ServiceIdentifier"/> with an instance.
    /// </summary>
    public interface IServiceResolver
    {
        /// <summary>
        /// Gets the identifier for the service to be resolved.
        /// </summary>
        ServiceIdentifier Identifier { get; }

        /// <summary>
        /// Resolves the requested service using the supplied <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider containing the necessary information to resolve the service.</param>
        /// <returns></returns>
        object Resolve(IServiceProvider serviceProvider);
    }
}
