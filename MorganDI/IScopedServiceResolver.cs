namespace MorganDI
{
    /// <summary>
    /// Represents a resolver that is bound by the <see cref="IServiceProvider"/>'s scope.
    /// </summary>
    public interface IScopedServiceResolver : IServiceResolver
    {
        /// <summary>
        /// Gets the scope in which the service is defined.
        /// </summary>
        Scope Scope { get; }
    }
}
