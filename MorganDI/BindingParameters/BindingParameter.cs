using System;

namespace MorganDI.BindingParameters
{
    internal abstract class BindingParameter : IBindingParameter
    {
        public virtual string Name { get; }
        public virtual Type ParameterType { get; }

        protected BindingParameter(string name, Type type)
        {
            Name = name;
            ParameterType = type;
        }

        public abstract object Resolve(IServiceProvider container, Scope scope);
    }
}
