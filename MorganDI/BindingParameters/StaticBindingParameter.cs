using System;

namespace MorganDI.BindingParameters
{
    internal class StaticBindingParameter : BindingParameter
    {
        private readonly object _value;

        private StaticBindingParameter(string name, Type type, object value) : base(name, type)
        {
            if (!type.IsAssignableFrom(value?.GetType() ?? type))
                throw new ArgumentException($"Supplied value, '{value}', is not compatible with the requested parameter, '{name}'.", nameof(value));

            _value = value;
        }

        public override object Resolve(IServiceProvider _, Scope scope)
        {
            return _value;
        }

        public static StaticBindingParameter Create(IBindingParameter parameter, object value) =>
            new StaticBindingParameter(parameter.Name, parameter.ParameterType, value);
    }
}
