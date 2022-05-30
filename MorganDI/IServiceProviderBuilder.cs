namespace MorganDI
{
    public delegate void ServiceProviderBuilderEventHandler(IServiceProviderBuilder builder, IServiceProvider serviceProvider);
    public delegate void ServiceConfigurationDelegate(IServiceCollection serviceCollection);

    /// <summary>
    /// Provides functionality to configure and build an <see cref="IServiceProvider"/>.
    /// </summary>
    public interface IServiceProviderBuilder
    {
        /// <summary>
        /// Called before a service provider is built.
        /// </summary>
        event ServiceProviderBuilderEventHandler ServiceProviderBuildStarted;

        /// <summary>
        /// Called when a service provider build is complete.
        /// </summary>
        event ServiceProviderBuilderEventHandler ServiceProviderBuildComplete;

        /// <summary>
        /// Processes the registered configurations to build the <see cref="IServiceProvider"/>.
        /// </summary>
        IServiceProvider Build();

        /// <summary>
        /// Register the supplied delegate to configure the <see cref="IServiceCollection"/>.
        /// </summary>
        IServiceProviderBuilder RegisterServiceConfiguration(ServiceConfigurationDelegate configurationDelegate);
    }
}
