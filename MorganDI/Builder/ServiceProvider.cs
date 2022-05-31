using System;
using System.Collections.Generic;

namespace MorganDI
{
    internal class ServiceProvider : IServiceProvider
    {
        // Provides a decorated version of the service provider to use during instantiation to track the requested services and catch circular references.
        private class ServiceProviderRecursionInstance : IServiceProvider
        {
            private readonly ServiceProvider _serviceProvider;
            private readonly Stack<ServiceIdentifier> _resolutionStack;

            public ServiceProviderRecursionInstance(ServiceProvider serviceProvider, Stack<ServiceIdentifier> resolutionStack)
            {
                _serviceProvider = serviceProvider;
                _resolutionStack = resolutionStack;
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

            public object Resolve(ServiceIdentifier identifier) => _serviceProvider.ResolveInternal(identifier, this, _resolutionStack);

            public bool ServiceExists(ServiceIdentifier identifier, Scope scope) => _serviceProvider.ServiceExists(identifier, scope);

            public void TeardownScene() => _serviceProvider.TeardownScene();
        }

        private readonly object _lock = new object();

        private bool _singletonsInitialized = false;
        private readonly Dictionary<ServiceIdentifier, DependencyNode> _allDependencyNodes = new Dictionary<ServiceIdentifier, DependencyNode>();
        private readonly HashSet<DependencyNode> _sceneContainer = new HashSet<DependencyNode>();

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

        public object Resolve(ServiceIdentifier identifier) => ResolveInternal(identifier);

        private object ResolveInternal(ServiceIdentifier identifier, IServiceProvider serviceProvider = null, Stack<ServiceIdentifier> resolutionStack = null)
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
                if (resolutionStack == null)
                {
                    resolutionStack = new Stack<ServiceIdentifier>();
                    serviceProvider = new ServiceProviderRecursionInstance(this, resolutionStack);
                }

                if (resolutionStack.Contains(identifier))
                    throw new InvalidDependencyDefinitionException($"Cyclical reference encountered, unable to resolve requested type '{resolutionStack.ToArray()[resolutionStack.Count-1]}'.");

                resolutionStack.Push(identifier);
                instance = node.Resolver.Resolve(serviceProvider);
                resolutionStack.Pop();

                if (node.Scope != Scope.Transient)
                {
                    // The instance isn't transient, store it for future resolutions

                    node.Instance = instance;
                    node.IsDisposable = instance is IDisposable;
                    node.IsResolved = true;

                    if (node.Scope == Scope.Scene)
                        _sceneContainer.Add(node);
                }

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
                        // Node hasn't been resolved
                        if (!node.IsResolved || node.Instance == null)
                            continue;

                        // Disposable types need to be be properly disposed of
                        if (node.IsDisposable)
                            ((IDisposable)node.Instance).Dispose();

                        // Reset the instance data
                        node.Instance = null;
                        node.IsDisposable = false;
                        node.IsResolved = false;
                    }
                }

            OnSceneTeardownComplete();
        }

        internal void InitializeSingletons()
        {
            if (!_singletonsInitialized)
                lock (_lock)
                    if (!_singletonsInitialized)
                    {
                        List<DependencyNode> sortedNodes = new List<DependencyNode>();

                        // Find all singleton nodes and add them to the singleton container
                        foreach (DependencyNode node in _allDependencyNodes.Values)
                        {
                            // Only add singletons, ignore duplicates due to aliases
                            if (node.Scope == Scope.Singleton && !sortedNodes.Contains(node))
                                sortedNodes.Add(node);
                        }

                        sortedNodes.Sort((a, b) => a.InitializationIndex.CompareTo(b.InitializationIndex));

                        IServiceProvider recursionInstance = new ServiceProviderRecursionInstance(this, new Stack<ServiceIdentifier>());
                        foreach (DependencyNode node in sortedNodes)
                        {
                            recursionInstance.Resolve(node.Identifier);
                            //if (node.IsResolved)
                            //    continue;

                            //node.Instance = node.Resolver.Resolve(new ServiceProviderRecursionInstance(this, new Stack<ServiceIdentifier>());
                            //node.IsResolved = true;
                            //node.IsDisposable = node.Instance != null && node is IDisposable;
                        }

                        _singletonsInitialized = true;
                    }
        }

        private void OnSceneTeardownRequested() => SceneTeardownRequested?.Invoke(this);
        private void OnSceneTeardownComplete() => SceneTeardownComplete?.Invoke(this);
        private void OnServiceRequested(ServiceIdentifier serviceIdentifier) => ServiceRequested?.Invoke(this, serviceIdentifier);
        private void OnServiceInstantiated(ServiceIdentifier serviceIdentifier, object value) => ServiceInstantiated?.Invoke(this, serviceIdentifier, value);
        private void OnServiceResolved(ServiceIdentifier serviceIdentifier, object value) => ServiceResolved?.Invoke(this, serviceIdentifier, value);
    }
}
