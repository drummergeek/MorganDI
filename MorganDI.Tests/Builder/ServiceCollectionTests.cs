using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace MorganDI.Tests.Builder
{
    public class ServiceCollectionTests
    {
        private const string SERVICE_NAME = "service";

        [Test]
        public void Contains_MissingService_False()
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                Assert.IsFalse(serviceCollection.Contains(ServiceIdentifier.Create<IServiceA>(), Scope.Singleton));
            });
        }

        [TestCase(null, false, TestName = "Contains_DefaultMissingWhileOtherExists_False")]
        [TestCase(SERVICE_NAME, true, TestName = "Contains_NamedExists_True")]
        [TestCase("Other", false, TestName = "Contains_NamedMissingWhileOtherExists_False")]
        public void Contains_MultipleIdentifiers(string name, bool expectedResult)
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                serviceCollection.AddSingletonService<IServiceA, ServiceA>(SERVICE_NAME);
                Assert.AreEqual(expectedResult, serviceCollection.Contains(ServiceIdentifier.Create<IServiceA>(name), Scope.Singleton));
            });
        }

        [Test]
        public void Contains_ServiceMissingForAlias_False()
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                serviceCollection.AddServiceAlias<IServiceA, ServiceA>();
                Assert.IsFalse(serviceCollection.Contains(ServiceIdentifier.Create<IServiceA>(), Scope.Singleton));

                // Adding the missing service to prevent the configuration being invalid for the rest of the build.
                serviceCollection.AddSingletonService<ServiceA>();
            });
        }

        [Test]
        public void Contains_AllValidScopeAndNamePermutations(
            [Values(null, SERVICE_NAME)] string serviceName,
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope serviceScope,
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope checkScope)
        {
            // Only when the check scope is at or higher than the service should it return true.
            bool expectedResult = checkScope >= serviceScope;

            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                serviceCollection.AddServiceToScope<IServiceA, ServiceA>(serviceScope, serviceName);
                Assert.AreEqual(expectedResult, serviceCollection.Contains(ServiceIdentifier.Create<IServiceA>(serviceName), checkScope));
            });
        }

        [Test]
        public void Get_ServiceExists_Success(
            [Values(null, SERVICE_NAME)]string name, 
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)]Scope scope)
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                var identifier = ServiceIdentifier.Create<IServiceA>(name);
                serviceCollection.AddServiceToScope<IServiceA, ServiceA>(scope, name);
                var registration = serviceCollection.Get(identifier);
                Assert.IsNotNull(registration);
                Assert.AreEqual(identifier, registration.Identifier);
                Assert.AreEqual(scope, registration.Scope);
            });
        }

        [Test]
        public void Get_ServiceMissing_KeyNotFoundException()
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                Assert.Throws<KeyNotFoundException>(() => serviceCollection.Get(ServiceIdentifier.Create<IServiceA>()));
            });
        }

        [Test]
        public void Add_NullService_ArgumentNullException()
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                Assert.Throws<ArgumentNullException>(() => serviceCollection.Add(null));
            });
        }

        [Test]
        public void Add_ServiceAlreadyExists_ArgumentException()
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                serviceCollection.AddSingletonService<IServiceA, ServiceA>();
                Assert.Throws<ArgumentException>(() => serviceCollection.AddSingletonService<IServiceA, ServiceA>());
            });
        }
    }
}
