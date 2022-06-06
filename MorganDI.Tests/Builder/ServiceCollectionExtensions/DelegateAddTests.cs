using MorganDI.Builder;
using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;

namespace MorganDI.Tests.Builder.ServiceCollectionExtensions
{
    public class DelegateAddTests
    {
        private const string SERVICE_NAME = "service";

        private void AddDelegate_Success(
            string name,
            Scope scope,
            Action<ServiceCollection, Scope, string, ServiceDelegate<IServiceA>> action = null
        )
        {
            ServiceDelegate<IServiceA> serviceDelegate = _ => default;
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                action(serviceCollection, scope, name, serviceDelegate);

                var identifier = ServiceIdentifier.Create<IServiceA>(name);
                Assert.IsTrue(serviceCollection.Contains(identifier, scope));
                var registration = serviceCollection.Get(identifier);

                Assert.IsNotNull(registration);
                Assert.AreEqual(scope, registration.Scope);
                Assert.IsInstanceOf<DelegateServiceRegistration<IServiceA>>(registration);
                var delegateRegistration = (DelegateServiceRegistration<IServiceA>)registration;
                Assert.AreEqual(serviceDelegate, delegateRegistration.ServiceDelegate);
            });
        }

        [Test]
        public void AddServiceDelegateToScope_WithoutNameArgument_Success(
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope scope
        ) => AddDelegate_Success(
                null,
                scope,
                (sc, s, n, sd) => sc.AddServiceDelegateToScope<IServiceA>(s, sd));

        [Test]
        public void AddServiceDelegateToScope_WithNameArgument_Success(
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope scope,
            [Values(null, SERVICE_NAME)] string name
        ) => AddDelegate_Success(
                name,
                scope,
                (sc, s, n, sd) => sc.AddServiceDelegateToScope<IServiceA>(s, n, sd));

        [Test]
        public void AddSingletonServiceDelegate_WithoutNameArgument_Success() =>
            AddDelegate_Success(
                null,
                Scope.Singleton,
                (sc, s, n, sd) => sc.AddSingletonServiceDelegate<IServiceA>(sd));

        [Test]
        public void AddSingletonServiceDelegate_WithNameArgument_Success(
            [Values(null, SERVICE_NAME)] string name
        ) => AddDelegate_Success(
                name,
                Scope.Singleton,
                (sc, s, n, sd) => sc.AddSingletonServiceDelegate<IServiceA>(n, sd));

        [Test]
        public void AddSceneServiceDelegate_WithoutNameArgument_Success() =>
            AddDelegate_Success(
                null,
                Scope.Scene,
                (sc, s, n, sd) => sc.AddSceneServiceDelegate<IServiceA>(sd));

        [Test]
        public void AddSceneServiceDelegate_WithNameArgument_Success(
            [Values(null, SERVICE_NAME)] string name
        ) => AddDelegate_Success(
                name,
                Scope.Scene,
                (sc, s, n, sd) => sc.AddSceneServiceDelegate<IServiceA>(n, sd));

        [Test]
        public void AddTransientServiceDelegate_WithoutNameArgument_Success() =>
            AddDelegate_Success(
                    null,
                    Scope.Transient,
                    (sc, s, n, sd) => sc.AddTransientServiceDelegate<IServiceA>(sd));

        [Test]
        public void AddTransientServiceDelegate_WithNameArgument_Success(
            [Values(null, SERVICE_NAME)] string name
        ) => AddDelegate_Success(
                name,
                Scope.Transient,
                (sc, s, n, sd) => sc.AddTransientServiceDelegate<IServiceA>(n, sd));

        [Test]
        public void AddServiceDelegateToScope_NullServiceCollection_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;
            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddServiceDelegateToScope<IServiceA>(Scope.Singleton, _ => default));
        }

        [Test]
        public void AddServiceDelegateToScope_NullServiceDelegate_ArgumentNullException()
        {
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                Assert.Throws<ArgumentNullException>(() => serviceCollection.AddServiceDelegateToScope<IServiceA>(Scope.Singleton, null));
            });
        }
    }
}
