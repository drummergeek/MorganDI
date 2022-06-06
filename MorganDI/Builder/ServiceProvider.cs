using System;
using System.Collections.Generic;

namespace MorganDI.Builder
{
    internal class ServiceProvider : IServiceProvider
    {
        // Provides a decorated version of the service provider to use during instantiation to track the requested services and catch circular references.
        private class ServiceProviderRecursionInstance : IServiceProvider
        {
            private readonly ServiceProvider _serviceProvider;

            public Stack<ServiceIdentifier> ResolutionStack { get; } = new Stack<ServiceIdentifier>();

            public ServiceProviderRecursionInstance(ServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public event ServiceProviderEventHandler InitializeRequested
            {
                add => _serviceProvider.InitializeRequested += value;
                remove => _serviceProvider.InitializeRequested -= value;
            }

            public event ServiceProviderEventHandler InitializeComplete
            {
                add => _serviceProvider.InitializeComplete += value;
                remove => _serviceProvider.InitializeComplete -= value;
            }

            public event ServiceProviderEventHandler SceneTeardownRequested
            {
                add => _serviceProvider.SceneTeardownRequested += value;
                remove => _serviceProvider.SceneTeardownRequested -= value;
            }

            public event ServiceProviderEventHandler SceneTeardownComplete
            {
                add => _serviceProvider.SceneTeardownComplete += value;
                remove => _serviceProvider.SceneTeardownComplete -= value;
            }

            public event ServiceRequestedEventHandler ServiceRequested
            {
                add => _serviceProvider.ServiceRequested += value;
                remove => _serviceProvider.ServiceRequested -= value;
            }

            public event ServiceResolvedEventHandler ServiceInstantiated
            {
                add => _serviceProvider.ServiceInstantiated += value;
                remove => _serviceProvider.ServiceInstantiated -= value;
            }

            public event ServiceResolvedEventHandler ServiceResolved
            {
                add => _serviceProvider.ServiceResolved += value;
                remove => _serviceProvider.ServiceResolved -= value;
            }

            public object Resolve(ServiceIdentifier identifier) => _serviceProvider.ResolveInternal(identifier, this);

            public bool ServiceExists(ServiceIdentifier identifier, Scope scope) => _serviceProvider.ServiceExists(identifier, scope);

            public void TeardownScene() => _serviceProvider.TeardownScene();

            public IServiceProvider Initialize() => _serviceProvider.Initialize();
        }

        private readonly object _lock = new object();

        private bool _singletonsInitialized = false;
        private readonly Dictionary<ServiceIdentifier, DependencyNode> _allDependencyNodes = new Dictionary<ServiceIdentifier, DependencyNode>();
        private readonly HashSet<DependencyNode> _sceneContainer = new HashSet<DependencyNode>();

        public event ServiceProviderEventHandler InitializeRequested;
        public event ServiceProviderEventHandler InitializeComplete;
        public event ServiceProviderEventHandler SceneTeardownRequested;
        public event ServiceProviderEventHandler SceneTeardownComplete;
        public event ServiceRequestedEventHandler ServiceRequested;
        public event ServiceResolvedEventHandler ServiceInstantiated;
        public event ServiceResolvedEventHandler ServiceResolved;

        internal Dictionary<ServiceIdentifier, DependencyNode> AllNodes => _allDependencyNodes;

        public bool ServiceExists(ServiceIdentifier identifier, Scope scope)
        {
            return _allDependencyNodes.TryGetValue(identifier, out DependencyNode node) &&
                node.Scope <= scope;
        }

        public object Resolve(ServiceIdentifier identifier)
        {
            Initialize(); // Ensure that the container has been initialized before resolving.

            return ResolveInternal(identifier);
        }

        private object ResolveInternal(ServiceIdentifier identifier, ServiceProviderRecursionInstance serviceProvider = null)
        {
            OnServiceRequested(identifier);

            if (!_allDependencyNodes.TryGetValue(identifier, out DependencyNode node))
                throw new ArgumentException($"No service found that matches the supplied identifier, '{identifier}'.", nameof(identifier));

            object instance;

            // No need to check anything, we've already resolved it.
            if (node.IsResolved)
            {
                instance = node.Instance;
            }
            else
            {
                if (serviceProvider != null && serviceProvider.ResolutionStack.Contains(identifier))
                    throw new InvalidDependencyDefinitionException(
                        $"Cyclical reference encountered, unable to resolve requested type '{serviceProvider.ResolutionStack.ToArray()[serviceProvider.ResolutionStack.Count - 1]}'.");

                serviceProvider = serviceProvider ?? new ServiceProviderRecursionInstance(this);

                serviceProvider.ResolutionStack.Push(identifier);
                instance = node.Resolve(serviceProvider);
                serviceProvider.ResolutionStack.Pop();

                if (node.Scope == Scope.Scene)
                    _sceneContainer.Add(node);

                OnServiceInstantiated(identifier, instance);
            }

            OnServiceResolved(identifier, instance);
            return instance;
        }

        public void TeardownScene()
        {
            OnSceneTeardownRequested();

            if (_sceneContainer.Count > 0)
                lock (_lock)
                    if (_sceneContainer.Count > 0)
                    {
                        List<DependencyNode> sortedNodes = new List<DependencyNode>(_sceneContainer);
                        // Sort the nodes in reverse order, so that the last instantiated is the first destroyed.
                        sortedNodes.Sort((a, b) => b.InitializationIndex.CompareTo(a.InitializationIndex));

                        foreach (DependencyNode node in sortedNodes)
                        {
                            node.Destroy();
                        }
                    }

            OnSceneTeardownComplete();
        }

        public IServiceProvider Initialize()
        {
            if (!_singletonsInitialized)
                lock (_lock)
                    if (!_singletonsInitialized)
                    {
                        OnInitializeRequested();

                        List<DependencyNode> sortedNodes = new List<DependencyNode>();

                        // Find all singleton nodes and add them to the singleton container
                        foreach (DependencyNode node in _allDependencyNodes.Values)
                        {
                            // Only add singletons, ignore duplicates due to aliases
                            if (node.Scope == Scope.Singleton && !sortedNodes.Contains(node))
                                sortedNodes.Add(node);
                        }

                        sortedNodes.Sort((a, b) => a.InitializationIndex.CompareTo(b.InitializationIndex));

                        IServiceProvider recursionInstance = new ServiceProviderRecursionInstance(this);
                        foreach (DependencyNode node in sortedNodes)
                        {
                            recursionInstance.Resolve(node.Identifier);
                        }

                        _singletonsInitialized = true;

                        OnInitializeComplete();
                    }

            return this;
        }

        private void OnInitializeRequested() => InitializeRequested?.Invoke(this);
        private void OnInitializeComplete() => InitializeComplete?.Invoke(this);
        private void OnSceneTeardownRequested() => SceneTeardownRequested?.Invoke(this);
        private void OnSceneTeardownComplete() => SceneTeardownComplete?.Invoke(this);
        private void OnServiceRequested(ServiceIdentifier serviceIdentifier) => ServiceRequested?.Invoke(this, serviceIdentifier);
        private void OnServiceInstantiated(ServiceIdentifier serviceIdentifier, object value) => ServiceInstantiated?.Invoke(this, serviceIdentifier, value);
        private void OnServiceResolved(ServiceIdentifier serviceIdentifier, object value) => ServiceResolved?.Invoke(this, serviceIdentifier, value);

        ~ServiceProvider()
        {
            if (_allDependencyNodes.Count > 0)
                lock (_lock)
                    if (_allDependencyNodes.Count > 0)
                    {
                        List<DependencyNode> sortedNodes = new List<DependencyNode>(_allDependencyNodes.Values);
                        // Sort the nodes in reverse order, so that the last instantiated is the first destroyed.
                        sortedNodes.Sort((a, b) => b.InitializationIndex.CompareTo(a.InitializationIndex));

                        foreach (DependencyNode node in sortedNodes)
                        {
                            node.Destroy();
                        }
                    }
        }
    }
}
