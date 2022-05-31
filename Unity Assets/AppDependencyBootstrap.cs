using MorganDI;

internal static class AppDependencyBootstrap
{
	public static IServiceProvider BuildContainer(IServiceProviderBuilder builder)
	{
		IServiceProvider serviceProvider = builder
			.RegisterServiceConfiguration(ConfigureCoreServices)
		//	.RegisterServiceConfiguration(MySystemBootstrapClass.ConfigureServices)
			.Register
			.Build();

		return serviceProvider;
	}

	private static void ConfigureCoreServices(IServiceCollection serviceCollection)
	{
		// Add core service registrations here
	}
}
