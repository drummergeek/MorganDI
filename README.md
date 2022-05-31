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
* ~~MonoBehvaiour examples for direct usage in Unity.~~ Added!
* Full test suite for the default implementation.
* More complete documentation.

## Example 
The following example creates a service provider builder, then passes it to a container configuration method, which then has the registers a method containing a selection of services. The configuration method then builds and returns the service provider.

	public static class App
	{
		private readonly IServiceProvider _serviceProvider;
		public IServiceProvider ServiceProvider => _serviceProvider;

		public static void Initialize()
		{
		    IServiceProvider serviceProvider =
			    BuildContainer(new ServiceProviderBuilder());
		}
	
		private static IServiceProvider BuildContainer(IServiceProviderBuilder builder)
		{
			IServiceProvider serviceProvider = builder
				.RegisterServiceConfiguration(ConfigureServices)
				.Build();
		
			return serviceProvider;
		}
	
		private static void ConfigureServices(IServiceCollection serviceCollection)
		{
			serviceCollection
				.AddStaticInstance<IServiceA>(new ServiceA())
				.AddSingletonService<ServiceBC, ServiceBC)
				.AddServiceAlias<IServiceB, ServiceBC>()
				.AddServiceAlias<IServiceC, ServiceBC)()
				.AddSingletonService<IServiceD, ServiceD>("direct")
				.AddSingletonService<IServiceD, ServiceDDecorator>()
				.BindParameterToService<IServiceD>("instance", "direct")
				.BindParameterToValue("cacheLength", new TimeSpan(1, 0, 0))
				.AddTransientService<IServiceE>(p => ServiceEFactory.Create())
				.AddSceneService<IServiceF, ServiceF>()
				.BindParameterToDelegate<string>("sceneName", p => Scene.GetSceneName());
		}
	}

Resolving a service from the container can be done directly (for MonoBehaviours, since we can't do constructor injection in that scenario) with the following method example.

	App.ServiceProvider.Resolve<IServiceA>();
		
or

	App.ServiceProvider.Resolve<IServiceD>("instance");

The scene scope can be cleared during the OnDestroy method of the container component in the scene hierarchy by calling the following line.

	App.ServiceProvider.TeardownScene(); 