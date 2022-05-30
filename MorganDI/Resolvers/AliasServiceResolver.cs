using System;

namespace MorganDI.Resolvers
{
    internal class AliasServiceResolver : ServiceResolver, IAliasServiceResolver
    {
        public AliasServiceResolver(ServiceIdentifier identifier, ServiceIdentifier rootIdentifier) : base(identifier)
        {
            if (!identifier.Type.IsAssignableFrom(rootIdentifier.Type))
                throw new ArgumentException($"Root service '{rootIdentifier}' is not assignable to the requested service '{identifier}'.", nameof(rootIdentifier));

            ServiceIdentifier = rootIdentifier;
        }

        public ServiceIdentifier ServiceIdentifier { get; }

        public override object Resolve(IServiceProvider container)
        {
            return container.Resolve(ServiceIdentifier);
        }
    }
}
