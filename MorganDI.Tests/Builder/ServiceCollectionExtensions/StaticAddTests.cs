using MorganDI.Builder;
using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;

namespace MorganDI.Tests.Builder.ServiceCollectionExtensions
{
    public class StaticAddTests
    {
        private const string SERVICE_NAME = "service";

        private void AddStatic_Success(
            string name,
            bool withInstance,
            Action<ServiceCollection, string, IServiceA> action = null)
        {
            IServiceA instance = withInstance ? new ServiceA() : null;
            ServiceCollectionTestHelper.RunServiceCollectionTest(serviceCollection =>
            {
                action(serviceCollection, name, instance);

                var identifier = ServiceIdentifier.Create<IServiceA>(name);
                Assert.IsTrue(serviceCollection.Contains(identifier, Scope.Singleton));
                var registration = serviceCollection.Get(identifier);

                Assert.IsNotNull(registration);
                Assert.AreEqual(Scope.Singleton, registration.Scope);
                Assert.IsInstanceOf<StaticServiceRegistration>(registration);
                var staticRegistration = (StaticServiceRegistration)registration;
                Assert.AreEqual(instance, staticRegistration.Instance);
            });
        }

        [Test]
        public void AddStaticInstance_WithoutNameArgument_Success(
            [Values(true, false)] bool withInstance
        ) => AddStatic_Success(
                null,
                withInstance,
                (sc, n, i) => sc.AddStaticInstance<IServiceA>(i));

        [Test]
        public void AddStaticInstance_WithNameArgument_Success(
            [Values(null, SERVICE_NAME)] string name,
            [Values(true, false)] bool withInstance
        ) => AddStatic_Success(
                null,
                withInstance,
                (sc, n, i) => sc.AddStaticInstance<IServiceA>(n, i));

        [Test]
        public void AddStaticInstance_NullServiceCollection_ArgumentNullException()
        {
            IServiceCollection serviceCollection = null;
            Assert.Throws<ArgumentNullException>(() => serviceCollection.AddStaticInstance<IServiceA>(null));
        }
    }
}
