using System;
using System.Collections.Generic;

namespace MorganDI.Builder
{
    /// <summary>
    /// Implementation of the <see cref="IServiceProviderBuilder"/>. Uses reflection to construct new instances.
    /// </summary>
    public class ServiceProviderBuilder : IServiceProviderBuilder
    {
        private readonly List<ServiceConfigurationDelegate> _configurationDelegates = new List<ServiceConfigurationDelegate>();

        /// <inheritdoc cref="IServiceProviderBuilder.ServiceProviderBuildStarted"/>
        public event ServiceProviderBuilderEventHandler ServiceProviderBuildStarted;

        /// <inheritdoc cref="IServiceProviderBuilder.ServiceProviderBuildComplete"/>
        public event ServiceProviderBuilderEventHandler ServiceProviderBuildComplete;

        /// <inheritdoc cref="IServiceProviderBuilder.Register(ServiceConfigurationDelegate)"/>
        public IServiceProviderBuilder Register(ServiceConfigurationDelegate configurationDelegate)
        {
            _configurationDelegates.Add(configurationDelegate ?? throw new ArgumentNullException(nameof(configurationDelegate)));

            return this;
        }

        /// <inheritdoc cref="IServiceProviderBuilder.Build"/>
        public IServiceProvider Build()
        {
            ServiceProvider serviceProvider = new ServiceProvider();

            OnServiceProviderBuildStarted(serviceProvider);

            // Don't bother running the build process if there are no delegates configured.
            if (_configurationDelegates.Count > 0)
            {
                ServiceCollection serviceCollection = ConfigureServiceCollection();
                BuildServiceProvider(serviceProvider, serviceCollection);
                GraphDependencies(serviceProvider);
            }

            OnServiceProviderBuildComplete(serviceProvider);

            return serviceProvider;
        }

        private ServiceCollection ConfigureServiceCollection()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            // Configure the service collection
            foreach (ServiceConfigurationDelegate configurationDelegate in _configurationDelegates)
                configurationDelegate(serviceCollection);

            return serviceCollection;
        }

        private void BuildServiceProvider(ServiceProvider serviceProvider, ServiceCollection serviceCollection)
        {
            List<(ServiceIdentifier Alias, ServiceIdentifier Service)> aliases = new List<(ServiceIdentifier Alias, ServiceIdentifier Service)>();

            // Build out Dependency Nodes from Service Collection
            foreach (ServiceRegistration service in serviceCollection)
            {
                if (service is AliasServiceRegistration alias)
                {
                    // Alias Dependency, just mark it down for now, we'll add it after all of the others are added
                    aliases.Add((service.Identifier, alias.Service));
                    continue;
                }

                DependencyNode node;

                if (service is FactoryServiceRegistration factoryService)
                {
                    // Construct a factory for this service
                    ReflectionServiceFactory factory = new ReflectionServiceFactory(factoryService.ServiceType);

                    node = new FactoryDependencyNode(factoryService, factory);
                }
                else
                {
                    node = new DependencyNode(service);
                }

                serviceProvider.AllNodes.Add(service.Identifier, node);
            }

            // Add the aliases
            foreach ((ServiceIdentifier Alias, ServiceIdentifier Service) in aliases)
            {
                if (!serviceProvider.AllNodes.ContainsKey(Service))
                    throw new InvalidDependencyDefinitionException($"Service '{Service}' not found for alias '{Alias}'");

                DependencyNode node = serviceProvider.AllNodes[Service];
                node.Aliases.Add(Alias);

                serviceProvider.AllNodes.Add(Alias, node);
            }
        }

        private void GraphDependencies(ServiceProvider serviceProvider)
        {
            Queue<ServiceIdentifier> dependencyQueue = new Queue<ServiceIdentifier>();

            int totalServiceCount = 0;

            // Determine Edge Count and Dependants
            foreach (KeyValuePair<ServiceIdentifier, DependencyNode> node in serviceProvider.AllNodes)
            {
                // Don't process aliases
                if (node.Key != node.Value.Identifier)
                    continue;

                totalServiceCount++;

                // Only factory nodes can have edges
                if (!(node.Value is FactoryDependencyNode factoryNode))
                {
                    dependencyQueue.Enqueue(node.Key);
                    continue;
                }

                // Determine the edge count and register as a dependant on all dependencies
                factoryNode.Initialize(serviceProvider);

                // Determine if we have any edges to worry about, enqueue if we don't 
                if (factoryNode.EdgeCount == 0)
                {
                    dependencyQueue.Enqueue(node.Key);
                }
            }

            int currentInitializationIndex = 0;

            // Process all dependencies in the queue, there must be at least one at this point if the collection contains items, otherwise we have a circular dependency
            while (dependencyQueue.Count > 0)
            {
                ServiceIdentifier currentService = dependencyQueue.Dequeue();
                DependencyNode node = serviceProvider.AllNodes[currentService];
                node.InitializationIndex = currentInitializationIndex;
                currentInitializationIndex++;

                if (node.Dependants.Count == 0)
                    continue;

                foreach (ServiceIdentifier dependant in node.Dependants)
                {
                    DependencyNode dependantNode = serviceProvider.AllNodes[dependant];
                    dependantNode.EdgeCount--;
                    if (dependantNode.EdgeCount == 0)
                        dependencyQueue.Enqueue(dependant);
                }
            }

            // Ensure we processed all services
            if (totalServiceCount > currentInitializationIndex)
                throw new InvalidDependencyDefinitionException("Cyclical references found, unable to build container");
        }

        private void OnServiceProviderBuildStarted(IServiceProvider serviceProvider) => ServiceProviderBuildStarted?.Invoke(this, serviceProvider);
        private void OnServiceProviderBuildComplete(IServiceProvider serviceProvider) => ServiceProviderBuildComplete?.Invoke(this, serviceProvider);
    }
}
