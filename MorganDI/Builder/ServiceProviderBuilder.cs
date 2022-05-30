using System;
using System.Collections.Generic;

namespace MorganDI
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

        /// <inheritdoc cref="IServiceProviderBuilder.Build"/>
        public IServiceProvider Build()
        {
            ServiceProvider serviceProvider = new ServiceProvider();

            OnServiceProviderBuildStarted(serviceProvider);

            // Don't bother running the build process if there are no delegates configured.
            if (_configurationDelegates.Count > 0)
            {
                BuildServiceProvider(serviceProvider);
            }

            OnServiceProviderBuildComplete(serviceProvider);

            return serviceProvider;
        }

        /// <inheritdoc cref="IServiceProviderBuilder.RegisterServiceConfiguration(ServiceConfigurationDelegate)"/>
        public IServiceProviderBuilder RegisterServiceConfiguration(ServiceConfigurationDelegate configurationDelegate)
        {
            _configurationDelegates.Add(configurationDelegate ?? throw new ArgumentNullException(nameof(configurationDelegate)));

            return this;
        }

        private void BuildServiceProvider(ServiceProvider serviceProvider)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            // Configure the service collection
            foreach (var configurationDelegate in _configurationDelegates)
                configurationDelegate(serviceCollection);

            List<(ServiceIdentifier Alias, ServiceIdentifier Root)> aliases = new List<(ServiceIdentifier Alias, ServiceIdentifier Root)>();

            // Build out Dependency Nodes from Service Collection
            foreach (IServiceResolver service in serviceCollection)
            {
                if (service is IAliasServiceResolver aliasResolver)
                {
                    // Alias Dependency, just mark it down for now, we'll add it after all of the others are added
                    aliases.Add((service.Identifier, aliasResolver.ServiceIdentifier));
                    continue;
                }
                serviceProvider.AllNodes.Add(service.Identifier, new DependencyNode(service));
            }

            // Add the aliases
            foreach ((ServiceIdentifier Alias, ServiceIdentifier Root) alias in aliases)
            {
                if (!serviceProvider.AllNodes.ContainsKey(alias.Root))
                    throw new InvalidDependencyDefinitionException($"Root type of '{alias.Root}' not found for alias '{alias.Alias}'");

                DependencyNode rootNode = serviceProvider.AllNodes[alias.Root];
                rootNode.Aliases.Add(alias.Alias);

                serviceProvider.AllNodes.Add(alias.Alias, rootNode);
            }

            GraphDependencies(serviceProvider);

            serviceProvider.InitializeSingletons();
        }

        private void GraphDependencies(ServiceProvider serviceProvider)
        {
            Queue<ServiceIdentifier> dependencyQueue = new Queue<ServiceIdentifier>();

            // Determine Edge Count and Dependants
            foreach (KeyValuePair<ServiceIdentifier, DependencyNode> node in serviceProvider.AllNodes)
            {
                // Don't bother with aliases, they will be handled by their root entries
                if (node.Key != node.Value.Identifier)
                    continue;

                // Only parameterized service resolvers with parameters have edge counts, throw everything else into the queue
                if (!(node.Value.Resolver is IParameterizedServiceResolver resolver) || resolver.BindingParameters.Count == 0)
                {
                    dependencyQueue.Enqueue(node.Key);
                    continue;
                }

                foreach (IBindingParameter parameter in resolver.BindingParameters)
                {
                    // We only care about service binding parameters here.
                    if (!(parameter is IServiceBindingParameter serviceParameter))
                        continue;

                    // Ensure that the dependency exists
                    DependencyNode parameterNode = null;
                    DependencyNode outOfScopeNode = null;
                    foreach (ServiceIdentifier identifier in serviceParameter.Identifiers)
                    {
                        if (serviceProvider.AllNodes.TryGetValue(identifier, out parameterNode) && parameterNode.Scope > resolver.Scope)
                        {
                            // Remember the node as out of scope, reset the parameter node for better error messaging later.
                            outOfScopeNode = parameterNode;
                            parameterNode = null;
                        }

                        if (parameterNode != null)
                            break;
                    }


                    if (parameterNode == null && outOfScopeNode == null)
                    {
                        throw new InvalidDependencyDefinitionException(
                            $"Required service not found for '{node.Key}' constructor parameter '{parameter.Name}'.\r\n" +
                            string.Join("\r\n", serviceParameter.Identifiers));
                    }
                    else if (parameterNode == null)
                    {
                        throw new InvalidDependencyDefinitionException(
                            $"Service '{outOfScopeNode.Identifier}' has a higher scope ({outOfScopeNode.Scope}) than '{node.Key}' ({resolver.Scope}).");
                    }

                    // Ensure that the scope of the dependency is lower or equal to the scope of the service depending on it
                    if (parameterNode.Resolver is IScopedServiceResolver dependencyResolver && dependencyResolver.Scope > resolver.Scope)
                        throw new InvalidDependencyDefinitionException(
                            $"Service '{dependencyResolver.Identifier}' has a higher scope ({dependencyResolver.Scope}) than '{node.Key}' ({resolver.Scope}).");

                    // We have a dependency on another service, add to our edge count and add us as a dependant on the other service, as long as we haven't been added already via alias
                    if (!parameterNode.Dependants.Contains(node.Key))
                    {
                        node.Value.EdgeCount++;
                        parameterNode.Dependants.Add(node.Key);
                    }
                }
            }

            int currentInitializationIndex = 1;

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

            // Ensure all dependencies have an initialization index
            foreach (DependencyNode node in serviceProvider.AllNodes.Values)
            {
                if (node.EdgeCount > 0 || node.InitializationIndex < 1)
                    throw new InvalidDependencyDefinitionException("Cyclical references found, unable to build container");
            }
        }

        private void OnServiceProviderBuildStarted(IServiceProvider serviceProvider) => ServiceProviderBuildStarted?.Invoke(this, serviceProvider);
        private void OnServiceProviderBuildComplete(IServiceProvider serviceProvider) => ServiceProviderBuildComplete?.Invoke(this, serviceProvider);
    }
}
