using System.Collections.Generic;

namespace MorganDI
{
    /// <summary>
    /// Represents a service resolver that uses <see cref="IBindingParameter"/>s during resolution.
    /// </summary>
    public interface IParameterizedServiceResolver : IScopedServiceResolver
    {
        /// <summary>
        /// Gets the list of defined <see cref="IBindingParameter"/>s.
        /// </summary>
        IReadOnlyCollection<IBindingParameter> BindingParameters { get; }

        /// <summary>
        /// Defines a <see cref="IBindingParameter"/> on the resolver.
        /// </summary>
        void SetBindingParameter(IBindingParameter bindingParameter);

        /// <summary>
        /// Gets the <see cref="IBindingParameter"/> that matches the supplied name.
        /// </summary>
        /// <param name="name">The name of the binding parameter</param>.
        IBindingParameter GetBindingParameter(string name);
    }
}
