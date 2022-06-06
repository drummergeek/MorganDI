using MorganDI.Builder;
using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;

namespace MorganDI.Tests.Builder.ServiceCollectionExtensions
{
    public class FactoryAddTests
    {
        private const string SERVICE_NAME = "service";

        private void AddService_Success<TService>(
            string name,
            Scope scope,
            bool withConfiguration,
            Action<ServiceCollection, Scope, string, ServiceFactoryConfigurationHandler> action = null
        )
        {
            ServiceFactoryConfigurationHandler factoryConfigurationHandler =
                withConfiguration
                    ? (f => { })
                    : (ServiceFactoryConfigurationHandler)null;

            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                action(serviceCollection, scope, name, factoryConfigurationHandler);

                var identifier = ServiceIdentifier.Create<TService>(name);
                Assert.IsTrue(serviceCollection.Contains(identifier, scope));
                var registration = serviceCollection.Get(identifier);

                Assert.IsNotNull(registration);
                Assert.AreEqual(scope, registration.Scope);
                Assert.IsInstanceOf<FactoryServiceRegistration>(registration);
                var factoryRegistration = (FactoryServiceRegistration)registration;
                Assert.AreEqual(factoryConfigurationHandler, factoryRegistration.FactoryConfigurationHandler);
                Assert.AreEqual(typeof(ServiceA), factoryRegistration.ServiceType);
            });
        }

        [Test]
        public void AddServiceToScope_FactoryWithoutNameArgumentWithInstanceType_Success(
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope scope,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            null,
            scope,
            withConfiguration,
            (sc, s, n, fch) => sc.AddServiceToScope<IServiceA, ServiceA>(s, fch));

        [Test]
        public void AddServiceToScope_FactoryWithoutNameArgumentWithoutInstanceType_Success(
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope scope,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            null,
            scope,
            withConfiguration,
            (sc, s, n, fch) => sc.AddServiceToScope<ServiceA>(s, fch));

        [Test]
        public void AddServiceToScope_FactoryWithNameArgumentWithInstanceType_Success(
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope scope,
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            name,
            scope,
            withConfiguration,
            (sc, s, n, fch) => sc.AddServiceToScope<IServiceA, ServiceA>(s, n, fch));

        [Test]
        public void AddServiceToScope_FactoryWithNameArgumentWithoutInstanceType_Success(
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope scope,
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            name,
            scope,
            withConfiguration,
            (sc, s, n, fch) => sc.AddServiceToScope<ServiceA>(s, n, fch));

        [Test]
        public void AddSingletonService_FactoryWithoutNameArgumentWithInstanceType_Success(
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            null,
            Scope.Singleton,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSingletonService<IServiceA, ServiceA>(fch));

        [Test]
        public void AddSingletonService_FactoryWithoutNameArgumentWithoutInstanceType_Success(
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            null,
            Scope.Singleton,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSingletonService<ServiceA>(fch));

        [Test]
        public void AddSingletonService_FactoryWithNameArgumentWithInstanceType_Success(
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            name,
            Scope.Singleton,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSingletonService<IServiceA, ServiceA>(n, fch));

        [Test]
        public void AddSingletonService_FactoryWithNameArgumentWithoutInstanceType_Success(
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            name,
            Scope.Singleton,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSingletonService<ServiceA>(n, fch));

        [Test]
        public void AddSceneService_FactoryWithoutNameArgumentWithInstanceType_Success(
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            null,
            Scope.Scene,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSceneService<IServiceA, ServiceA>(fch));

        [Test]
        public void AddSceneService_FactoryWithoutNameArgumentWithoutInstanceType_Success(
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            null,
            Scope.Scene,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSceneService<ServiceA>(fch));

        [Test]
        public void AddSceneService_FactoryWithNameArgumentWithInstanceType_Success(
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            name,
            Scope.Scene,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSceneService<IServiceA, ServiceA>(n, fch));

        [Test]
        public void AddSceneService_FactoryWithNameArgumentWithoutInstanceType_Success(
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            name,
            Scope.Scene,
            withConfiguration,
            (sc, s, n, fch) => sc.AddSceneService<ServiceA>(n, fch));

        [Test]
        public void AddTransientService_FactoryWithoutNameArgumentWithInstanceType_Success(
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            null,
            Scope.Transient,
            withConfiguration,
            (sc, s, n, fch) => sc.AddTransientService<IServiceA, ServiceA>(fch));

        [Test]
        public void AddTransientService_FactoryWithoutNameArgumentWithoutInstanceType_Success(
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            null,
            Scope.Transient,
            withConfiguration,
            (sc, s, n, fch) => sc.AddTransientService<ServiceA>(fch));

        [Test]
        public void AddTransientService_FactoryWithNameArgumentWithInstanceType_Success(
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<IServiceA>(
            name,
            Scope.Transient,
            withConfiguration,
            (sc, s, n, fch) => sc.AddTransientService<IServiceA, ServiceA>(n, fch));

        [Test]
        public void AddTransientService_FactoryWithNameArgumentWithoutInstanceType_Success(
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withConfiguration
        ) => AddService_Success<ServiceA>(
            name,
            Scope.Transient,
            withConfiguration,
            (sc, s, n, fch) => sc.AddTransientService<ServiceA>(n, fch));

        [Test]
        public void AddServiceToScope_NullServiceCollection_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;
            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddServiceToScope<IServiceA, ServiceA>(Scope.Singleton));
        }
    }
}
