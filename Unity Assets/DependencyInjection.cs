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