namespace MorganDI
{
    /// <summary>
    /// Aliases one <see cref="MorganDI.ServiceIdentifier"/> to an existing, compatible service.
    /// </summary>
    public interface IAliasServiceResolver : IServiceResolver
    {
        /// <summary>
        /// Gets the identifier of the service this service will alias.
        /// </summary>
        ServiceIdentifier ServiceIdentifier { get; }
    }
}
