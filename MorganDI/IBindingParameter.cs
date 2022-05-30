using System;

namespace MorganDI
{
    /// <summary>
    /// Represents a parameter used during the construction of a service.
    /// </summary>
    public interface IBindingParameter
    {
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the <see cref="System.Type"/> of this parameter.
        /// </summary>
        Type ParameterType { get; }

        /// <summary>
        /// Resolves the value of this parameter.
        /// </summary>
        /// <param name="container">The <see cref="IServiceProvider"/> requesting the resolution.</param>
        /// <param name="scope">The <see cref="Scope"/> of the requesting service.</param>
        object Resolve(IServiceProvider container, Scope scope);
    }
}