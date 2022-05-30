using System;

namespace MorganDI
{
    /// <summary>
    /// Represents a configured service in a <see cref="IServiceCollection"/> or <see cref="IServiceProvider"/>.
    /// </summary>
    public struct ServiceIdentifier : IEquatable<ServiceIdentifier>
    {
        /// <summary>
        /// Constructs a new ServiceIdentifier for the supplied type with an optional name.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> defining the service.</param>
        /// <param name="name">The optional name of the service registration.</param>
        public ServiceIdentifier(Type type, string name = null)
        {
            Type = type;

            Name = string.IsNullOrEmpty(name)
                ? null
                : name.Trim();
        }

        /// <summary>
        /// Gets the <see cref="Type"/> defining the service.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the optional name of the service registration.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Creates a new ServiceIdentifier using a generic type parameter to specify the associated type.
        /// </summary>
        /// <typeparam name="TService">The <see cref="Type"/> defining the service.</typeparam>
        /// <param name="name">The optional name of the service registration.</param>
        public static ServiceIdentifier Create<TService>(string name = null) => new ServiceIdentifier(typeof(TService), name);

        /// <inheritdoc cref="Object.Equals(object)"/>
        public override bool Equals(object obj) => obj is ServiceIdentifier other && this.Equals(other);

        /// <inheritdoc cref="IEquatable{ServiceIdentifier}.Equals(ServiceIdentifier)"/>
        public bool Equals(ServiceIdentifier other) => Type == other.Type && Name == other.Name;

        /// <inheritdoc cref="Object.GetHashCode"/>
        public override int GetHashCode() => (Name, Type).GetHashCode();

        /// <inheritdoc cref="Object.ToString"/>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Name)
                ? $"{Type.Assembly.GetName().Name}::{Type.FullName}"
                : $"{Type.Assembly.GetName().Name}::{Type.FullName}::{Name}";
        }

        public static bool operator ==(ServiceIdentifier a, ServiceIdentifier b) => a.Equals(b);

        public static bool operator !=(ServiceIdentifier a, ServiceIdentifier b) => !(a==b);
    }
}
