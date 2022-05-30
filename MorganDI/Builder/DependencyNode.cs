using System;
using System.Collections.Generic;

namespace MorganDI
{
    internal class DependencyNode
    {
        public DependencyNode(IServiceResolver resolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            Scope = !(resolver is IScopedServiceResolver scopedResolver)
                ? Scope.Singleton
                : scopedResolver.Scope;
        }

        public IServiceResolver Resolver { get; }
        public Scope Scope { get; }
        public ServiceIdentifier Identifier => Resolver.Identifier;
        public List<ServiceIdentifier> Aliases { get; } = new List<ServiceIdentifier>();
        public List<ServiceIdentifier> Dependants { get; } = new List<ServiceIdentifier>();

        public int InitializationIndex { get; set; }
        public int EdgeCount { get; set; }

        public bool IsResolved { get; set; }
        public bool IsDisposable { get; set; }
        public object Instance { get; set; }

        public override string ToString()
        {
            return Identifier.ToString();
        }
    }
}
