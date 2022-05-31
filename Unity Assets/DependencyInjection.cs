using MorganDI;
using UnityEngine;

public class DependencyInjection : MonoBehaviour
{
	private void Awake()
	{
		DI.Initialize();
	}

	private void OnDestroy()
	{
		DI.ServiceProvider.TeardownScene();
	}
}

public static class DI
{
	private readonly static object _lock = new object();

	private static IServiceProvider _serviceProvider;
	public static IServiceProvider ServiceProvider => _serviceProvider;

	public static void Initialize()
	{
		if (_serviceProvider == null)
			lock (_lock)
				if (_serviceProvider == null)
				{
					_serviceProvider = AppDependencyBootstrap.BuildContainer(new ServiceProviderBuilder());
				}
	}
}