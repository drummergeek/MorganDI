using System;

namespace MorganDI.Resolvers
{
    internal class DelegateServiceResolver<TValue> : ServiceResolver, IDelegateServiceResolver
    {
        private readonly ServiceDelegate<TValue> _delegate;

        public virtual Scope Scope { get; }

        public DelegateServiceResolver(ServiceIdentifier identifier, Scope scope, ServiceDelegate<TValue> resolverDelegate) : base(identifier)
        {
            if (resolverDelegate == null)
                throw new ArgumentNullException(nameof(resolverDelegate));

            if (!identifier.Type.IsAssignableFrom(typeof(TValue)))
                throw new ArgumentException($"Delegate return type is not compatible with the supplied service, {identifier}", nameof(resolverDelegate));

            Scope = scope;
            _delegate = resolverDelegate;
        }

        public override object Resolve(IServiceProvider container)
        {
            return _delegate(container);
        }
    }
}
