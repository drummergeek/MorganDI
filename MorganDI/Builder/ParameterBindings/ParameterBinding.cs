using System;

namespace MorganDI.Builder.ParameterBindings
{
    internal abstract class ParameterBinding
    {
        public string Name { get; }
        public Type ParameterType { get; }

        protected ParameterBinding(string name, Type type)
        {
            Name = name;
            ParameterType = type;
        }

        public abstract object Resolve(IServiceProvider serviceProvider, Scope scope);
    }
}
