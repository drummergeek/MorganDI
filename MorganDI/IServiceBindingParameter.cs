using System.Collections.Generic;

namespace MorganDI
{
    /// <summary>
    /// Represents a <see cref="IBindingParameter"/> that resolves to a service defined in the <see cref="IServiceProvider"/>.
    /// </summary>
    public interface IServiceBindingParameter : IBindingParameter
    {
        /// <summary>
        /// Gets an enumeration, in order, of the identifiers to use for resolution of the parameter's value.
        /// </summary>
        IEnumerable<ServiceIdentifier> Identifiers { get; }
    }
}