using System;

namespace MorganDI.Resolvers
{
    internal class StaticServiceResolver : ServiceResolver
    {
        private object _service;

        public StaticServiceResolver(ServiceIdentifier identifier, object service) : base(identifier)
        {
            if (!identifier.Type.IsAssignableFrom(service?.GetType() ?? identifier.Type))
                throw new ArgumentException($"The type of the supplied value '{service.GetType()}' is not assignable to the supplied service '{identifier}'.", nameof(service));

            _service = service;
        }

        public override object Resolve(IServiceProvider _)
        {
            return _service;
        }
    }
}
