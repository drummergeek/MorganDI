using System.Collections.Generic;

namespace MorganDI.Builder
{
    internal sealed class FactoryDependencyNode : DependencyNode
    {
        private readonly FactoryServiceRegistration _service;
        private readonly ReflectionServiceFactory _factory;

        public override ServiceRegistration Service => _service;


        public FactoryDependencyNode(FactoryServiceRegistration service, ReflectionServiceFactory factory)
            : base(service)
        {
            _service = service;
            _factory = factory;
        }

        public void Initialize(ServiceProvider serviceProvider)
        {
            // Configure the factory, if a handler is provided
            _service.FactoryConfigurationHandler?.Invoke(_factory);

            HashSet<ServiceIdentifier> edges = _factory.GetRequiredServices(serviceProvider, Scope);
            EdgeCount = edges.Count;

            foreach(ServiceIdentifier edge in edges)
                serviceProvider.AllNodes[edge].Dependants.Add(Identifier);
        }


        protected override object ResolveInternal(IServiceProvider serviceProvider) => _factory.Instantiate(serviceProvider, Scope);
    }
}
