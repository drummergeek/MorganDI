using System;

namespace MorganDI.BindingParameters
{
    internal class DelegateBindingParameter<TValue> : BindingParameter, IDelegateBindingParameter
    {
        private readonly ServiceDelegate<TValue> _delegate;

        private DelegateBindingParameter(string name, Type type, ServiceDelegate<TValue> valueDelegate) : base(name, type)
        {
            if (valueDelegate == null)
                throw new ArgumentNullException(nameof(valueDelegate));

            if (!type.IsAssignableFrom(typeof(TValue)))
                throw new ArgumentException($"Delegate return type is not compatible with the requested parameter, {name}", nameof(valueDelegate));

            _delegate = valueDelegate;
        }

        public override object Resolve(IServiceProvider container, Scope scope)
        {
            return _delegate(container);
        }

        public static DelegateBindingParameter<TValue> Create(IBindingParameter parameter, ServiceDelegate<TValue> serviceDelegate) =>
            new DelegateBindingParameter<TValue>(parameter.Name, parameter.ParameterType, serviceDelegate);
    }
}
