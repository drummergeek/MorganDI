namespace MorganDI.Resolvers
{
    internal abstract class ServiceResolver : IServiceResolver
    {
        public ServiceIdentifier Identifier { get; }

        protected ServiceResolver(ServiceIdentifier identifier)
        {
            Identifier = identifier;
        }

        public abstract object Resolve(IServiceProvider container);
    }
}
