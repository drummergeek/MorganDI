using MorganDI.BindingParameters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MorganDI.Resolvers
{
    internal class ConstructorServiceResolver : ServiceResolver, IParameterizedServiceResolver
    {
        private readonly Dictionary<string, int> _bindingParameterIndexes = new Dictionary<string, int>();
        private readonly IBindingParameter[] _bindingParameters;
        private readonly ConstructorInfo _constructorInfo;

        public virtual IReadOnlyCollection<IBindingParameter> BindingParameters => _bindingParameters;
        public virtual Scope Scope { get; }

        public ConstructorServiceResolver(ServiceIdentifier identifier, Scope scope, Type instanceType) : base(identifier)
        {
            if (!identifier.Type.IsAssignableFrom(instanceType))
                throw new ArgumentException($"Supplied instance type '{instanceType}' is not assignable to the requested service '{identifier}'.", nameof(instanceType));
            
            Scope = scope;

            _constructorInfo = instanceType.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
            ParameterInfo[] parameterInfo = _constructorInfo.GetParameters();
            _bindingParameters = new IBindingParameter[parameterInfo.Length];

            for(int i = 0; i < parameterInfo.Length; i++)
            {
                var parameter = parameterInfo[i];
                _bindingParameterIndexes.Add(parameter.Name, i);
                _bindingParameters[i] = ServiceBindingParameter.Create(parameter);
            }
        }

        public virtual void SetBindingParameter(IBindingParameter bindingParameter)
        {
            if (bindingParameter == null)
                throw new ArgumentNullException(nameof(bindingParameter));
            if (!_bindingParameterIndexes.ContainsKey(bindingParameter.Name))
                throw new ArgumentException($"No matching parameter with name '{bindingParameter.Name}' found.", nameof(bindingParameter));

            _bindingParameters[_bindingParameterIndexes[bindingParameter.Name]] = bindingParameter;
        }

        public virtual IBindingParameter GetBindingParameter(string name)
        {
            if (!_bindingParameterIndexes.ContainsKey(name))
                throw new ArgumentException($"No matching parameter with name '{name}' found.", nameof(name));

            return _bindingParameters[_bindingParameterIndexes[name]];
        }

        public override object Resolve(IServiceProvider container)
        {
            object[] parameters = new object[_bindingParameters.Length];

            for(int i = 0; i < _bindingParameters.Length; i++)
            {
                parameters[i] = _bindingParameters[i].Resolve(container, Scope);
            }

            return _constructorInfo.Invoke(parameters);
        }
    }
}
