using MorganDI.Builder;
using System;

namespace MorganDI.Tests.Builder
{
    public static class ServiceCollectionTestHelper
    {
        public static void RunServiceCollectionTest(Action<ServiceCollection> testAction)
        {
            var builder = new ServiceProviderBuilder();
            builder.Register(sc =>
            {
                ServiceCollection serviceCollection = (ServiceCollection)sc;
                testAction(serviceCollection);
            });
            builder.Build();
        }
    }
}
