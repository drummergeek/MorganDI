using System;
using System.Collections.Generic;

namespace MorganDI
{
    internal class ServiceProvider : IServiceProvider
    {
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

        public object Resolve(ServiceIdentifier identifier)
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
                instance = node.Resolver.Resolve(this);

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
            if (_singletonsInitialized)
                return;

            lock (_lock)
            {
                if (_singletonsInitialized)
                    return;

                List<DependencyNode> sortedNodes = new List<DependencyNode>();

                // Find all singleton nodes and add them to the singleton container
                foreach (var node in _allDependencyNodes.Values)
                {
                    // Only add singletons, ignore duplicates due to aliases
                    if (node.Scope == Scope.Singleton && !sortedNodes.Contains(node))
                        sortedNodes.Add(node);
                }

                sortedNodes.Sort((a, b) => a.InitializationIndex.CompareTo(b.InitializationIndex));

                foreach (DependencyNode node in sortedNodes)
                {
                    if (node.IsResolved)
                        continue;

                    node.Instance = node.Resolver.Resolve(this);
                    node.IsResolved = true;
                    node.IsDisposable = node.Instance != null && node is IDisposable;
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
