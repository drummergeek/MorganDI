namespace MorganDI
{
    /// <summary>
    /// Defines the resolution scope of the service in a service provider.
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// One instance is created for the container.
        /// </summary>
        Singleton = 0,

        /// <summary>
        /// One instance is created for a scene, then destroyed upon the destruction of the scene.
        /// </summary>
        Scene = 1,

        /// <summary>
        /// A new instance is provided on each request.
        /// </summary>
        Transient = 2,
    }
}
