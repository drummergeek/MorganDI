
# Morgan DI
A dependency injection framework for Unity3D that mimics the style of Microsoft.Extensions.DependencyInjection.

Supports 3 Lifetime Scopes
* **Singleton** - Only one instance is created for the service for the lifetime of the container.
* **Scene** - Only one instance is created per Unity scene, disposed of during scene teardown
* **Transient** - A new instance is created per request.

## Features
* Supports named instances
* Four methods of service registration
	* **Constructor** - Automatically binds constructor parameters to the services in the container. Attempts to resolve via a named instance first, then the default instance unless configured otherwise.
	* **Static** - Bind a static instance to a service.
	* **Delegate** - Invokes a supplied delegate method to instantiate the service.
	*  **Alias** - Binds a service to a compatible service that is already registered.
* Configuration is decoupled from the building of the container allowing for other implementations based on scenario and platform. Default implementation uses reflection for constructor based instantiation.
* Extension methods included to allow for fluent style configuration.
* No dependence on Unity assemblies, allowing for testing and reuse outside of Unity3D.
* Events wired to the service provider and service provider builder to know when services are instantiating, and when they are requested.

## Coming Soon
* ~~Full test suite for the default implementation.~~ Done!
* More complete documentation.
* ~~MonoBehvaiour examples for direct usage in Unity.~~ Added!

## Example 
The following example creates a service provider builder, then passes it to a container configuration method in the application bootstrap class. It stores the container as a singleton for use in the MonoBehaviours. The static class providing the DI abstracts the calls away from the actual interface so that it is simpler to call and decouples the framework from the actual application logic. The OnDestroy method in the component will clear the scene scope in preparation for the next scene load. This component should have one instance on every scene that is going to utilize dependency injection.

	public class DependencyInjection : MonoBehaviour
	{
	    private void Awake()
	    {
	        DI.Initialize();
	    }

	    private void OnDestroy()
	    {
	        DI.TeardownScene();
	    }
	}

	public static class DI
	{
	    private readonly static object _lock = new object();

	    private static IServiceProvider _serviceProvider;

	    /// <summary>
	    /// Build the DI container if it is not yet available.
	    /// </summary>
	    public static void Initialize()
	    {
	        if (_serviceProvider == null)
	            lock (_lock)
	                // Ensure that something else didn't instantiate it before we acquired the lock
	                if (_serviceProvider == null) 
	                {
	                    _serviceProvider = AppDependencyBootstrap.BuildContainer(new ServiceProviderBuilder());
	                }
	    }
		
	    /// <inheritdoc cref="IServiceProvider.TeardownScene"/>
	    public static void TeardownScene()
	    {
	        if (_serviceProvider == null)
	            throw new InvalidOperationException("DI container not initialized!");
	
	        _serviceProvider.TeardownScene();
	    }
		
	    /// <summary>
	    /// Return an instance of the requested service.
	    /// </summary>
	    /// <typeparam name="TService">The type of the service requested.</typeparam>
	    /// <param name="name">The optional instance name of the service requested.</param>
	    public static TService Resolve<TService>(string name = null)
	    {
	        if (_serviceProvider == null)
	            throw new InvalidOperationException("DI container not initialized!");
	
	        return _serviceProvider.Resolve<TService>(name);
	    }
	}

The Bootstrap class below is what actually performs the configuration of the builder and returns the container.

	internal static class AppDependencyBootstrap
	{
	    public static IServiceProvider BuildContainer(IServiceProviderBuilder builder)
	    {
	        IServiceProvider serviceProvider = builder
	            .RegisterServiceConfiguration(ConfigureCoreServices)
	        //  .RegisterServiceConfiguration(MySystemBootstrapClass.ConfigureServices)
	            .Register
	            .Build();
	
	        return serviceProvider;
	    }
	
	    private static void ConfigureCoreServices(IServiceCollection serviceCollection)
	    {
	        // Add core service registrations here
	        serviceCollection
	            .AddStaticInstance<IServiceA>(new ServiceA())
	            .AddSingletonService<ServiceBC>()
	            .AddServiceAlias<IServiceB, ServiceBC>()
	            .AddServiceAlias<IServiceC, ServiceBC)()
	            .AddSingletonService<IServiceD, ServiceD>("direct")
	            .AddSingletonService<IServiceD, ServiceDDecorator>(x => x
	                .BindParameterToService<IServiceD>("instance", "direct")
	                .BindParameterToValue("cacheLength", new TimeSpan(1, 0, 0)))
	            .AddTransientService<IServiceE>(p => ServiceEFactory.Create())
	            .AddSceneService<IServiceF, ServiceF>(x => x
	                .BindParameterToDelegate<string>("sceneName", p => Scene.GetSceneName()));
	    }
	}


Resolving a service in a MonoBehviour is done by calling the Resolve method on the DI singleton. 

    public class MyComponent : MonoBehaviour
    {
        private IServiceA _serviceA;
        private IServiceD _serviceD;
    
        private void Awake()
        {
            _serviceA = DI.Resolve<IServiceA>();
            _serviceD = DI.Resolve<IServiceD>("direct");
        }
    }
