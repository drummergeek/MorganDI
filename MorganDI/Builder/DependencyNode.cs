using System;
using System.Collections.Generic;

namespace MorganDI.Builder
{
    internal class DependencyNode
    {
        private bool _isDisposable;

        public DependencyNode(ServiceRegistration service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));

            if (service is AliasServiceRegistration)
                throw new ArgumentException("Dependency nodes cannot be created from alias registrations.");
        }

        public virtual ServiceRegistration Service { get; }
        public Scope Scope => Service.Scope;
        public ServiceIdentifier Identifier => Service.Identifier;
        public List<ServiceIdentifier> Aliases { get; } = new List<ServiceIdentifier>();
        public List<ServiceIdentifier> Dependants { get; } = new List<ServiceIdentifier>();

        public int InitializationIndex { get; set; }
        public int EdgeCount { get; set; }
        public bool IsResolved { get; private set; }
        public object Instance { get; private set; }

        public object Resolve(IServiceProvider serviceProvider)
        {
            if (IsResolved)
                return Instance;

            object instance = ResolveInternal(serviceProvider);

            if (Scope != Scope.Transient)
            {
                Instance = instance;
                _isDisposable = instance != null && instance.GetType() is IDisposable;
                IsResolved = true;
            }

            return instance;
        }

        protected virtual object ResolveInternal(IServiceProvider serviceProvider)
        {
            object instance;

            if (Service is StaticServiceRegistration staticService)
                instance = staticService.Instance;
            else if (Service is DelegateServiceRegistration delegateService)
                instance = delegateService.Resolve(serviceProvider);
            else
                throw new InvalidDependencyDefinitionException("Unknown service registration type!");

            return instance;
        }

        public void Destroy()
        {
            if (!IsResolved)
                return;

            if (_isDisposable)
            {
                ((IDisposable)Instance).Dispose();
            }

            Instance = null;
            IsResolved = false;
        }

        public override string ToString()
        {
            return Identifier.ToString();
        }
    }
}
