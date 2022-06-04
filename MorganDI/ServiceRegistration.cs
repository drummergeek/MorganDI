namespace MorganDI
{
    /// <summary>
    /// Provides information about a service registered in an <see cref="IServiceCollection"/>.
    /// </summary>
    public abstract class ServiceRegistration
    {
        protected ServiceRegistration(ServiceIdentifier identifier, Scope scope)
        {
            Identifier = identifier;
            Scope = scope;
        }

        /// <summary>
        /// Returns the <see cref="ServiceIdentifier"/> of the registered service.
        /// </summary>
        public ServiceIdentifier Identifier { get; }

        /// <summary>
        /// Returns the <see cref="Scope"/> of the registered service.
        /// </summary>
        public Scope Scope { get; }
    }
}
