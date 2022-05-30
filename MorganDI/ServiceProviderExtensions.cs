namespace MorganDI
{
    public static class ServiceProviderExtensions
    {
        public static TService Resolve<TService>(this IServiceProvider container, string name = null)
        {
            return (TService)container.Resolve(ServiceIdentifier.Create<TService>(name));
        }
    }
}
