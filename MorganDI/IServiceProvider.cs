namespace MorganDI
{
    public delegate void ServiceProviderEventHandler(IServiceProvider sender);
    public delegate void ServiceRequestedEventHandler(IServiceProvider sender, ServiceIdentifier serviceIdentifier);
    public delegate void ServiceResolvedEventHandler(IServiceProvider sender, ServiceIdentifier serviceIdentifier, object serviceInstance);

    public interface IServiceProvider
    {
        /// <summary>
        /// Called when before a scene teardown is executed.
        /// </summary>        
        event ServiceProviderEventHandler SceneTeardownRequested;
        
        /// <summary>
        /// Called when a scene teardown is complete.
        /// </summary>
        event ServiceProviderEventHandler SceneTeardownComplete;

        /// <summary>
        /// Called when a service is requested, but before it is resolved.
        /// </summary>
        event ServiceRequestedEventHandler ServiceRequested;

        /// <summary>
        /// Called after a service is constructed.
        /// </summary>
        event ServiceResolvedEventHandler ServiceInstantiated;

        /// <summary>
        /// Called after a service is resolved.
        /// </summary>
        event ServiceResolvedEventHandler ServiceResolved;

        /// <summary>
        /// Return whether or not a service is defined at the specified scope or below.
        /// </summary>
        /// <param name="identifier">The type and optional instance name of the service requested</param>
        /// <param name="scope">The scope to search for the service</param>
        bool ServiceExists(ServiceIdentifier identifier, Scope scope);

        /// <summary>
        /// Return an instance of the requested service.
        /// </summary>
        /// <param name="identifier">The type and optional instance name of the service requested</param>
        object Resolve(ServiceIdentifier identifier);

        /// <summary>
        /// Destroy all instances existing in the current scene container.
        /// </summary>
        void TeardownScene();
    }
}
