using System;

namespace MorganDI.Builder.ParameterBindings
{
    internal sealed class DelegateParameterBinding<TValue> : ParameterBinding
    {
        private readonly ServiceDelegate<TValue> _delegate;

        private DelegateParameterBinding(string name, Type type, ServiceDelegate<TValue> valueDelegate) : base(name, type)
        {
            if (valueDelegate == null)
                throw new ArgumentNullException(nameof(valueDelegate));

            if (!type.IsAssignableFrom(typeof(TValue)))
                throw new ArgumentException($"Delegate return type is not compatible with the requested parameter, {name}", nameof(valueDelegate));

            _delegate = valueDelegate;
        }

        public override object Resolve(IServiceProvider serviceProvider, Scope scope) =>
            _delegate(serviceProvider);

        public static DelegateParameterBinding<TValue> Create(ParameterBinding parameter, ServiceDelegate<TValue> serviceDelegate) =>
            new DelegateParameterBinding<TValue>(parameter.Name, parameter.ParameterType, serviceDelegate);
    }
}
