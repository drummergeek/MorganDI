using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;

namespace MorganDI.Tests.Builder.ServiceCollectionExtensions
{
    public class AliasAddTests
    {
        private const string SERVICE_NAME = "service";

        [Test]
        public void AddServiceAlias_Success(
            [Values(null, SERVICE_NAME)] string aliasName,
            [Values(null, SERVICE_NAME)] string serviceName)
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                // Should be able to add an alias without the service existing.
                serviceCollection.AddServiceAlias<IServiceA, ServiceAB>(aliasName, serviceName);

                var aliasIdentifier = ServiceIdentifier.Create<IServiceA>(aliasName);
                var serviceIdentifier = ServiceIdentifier.Create<ServiceAB>(serviceName);

                // Adding the service to perform the contains check
                serviceCollection.AddSingletonService<ServiceAB>(serviceName);

                Assert.IsTrue(serviceCollection.Contains(aliasIdentifier, Scope.Singleton));

                var registration = serviceCollection.Get(aliasIdentifier);

                Assert.IsNotNull(registration);
                Assert.AreEqual(Scope.Singleton, registration.Scope);
                Assert.IsInstanceOf<AliasServiceRegistration>(registration);
                var aliasRegistration = (AliasServiceRegistration)registration;
                Assert.AreEqual(serviceIdentifier, aliasRegistration.Service);
            });
        }

        [Test]
        public void AddServiceAlias_AliasMatchesService_ArgumentException()
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                Assert.Throws<ArgumentException>(() => serviceCollection.AddServiceAlias<IServiceA, IServiceA>());
            });
        }

        [Test]
        public void AddServiceAlias_NullServiceCollection_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;
            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddServiceAlias<IServiceA, ServiceAB>(null));
        }
    }
}
