using MorganDI.Builder;
using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;

namespace MorganDI.Tests.Builder
{
    public class ServiceFactoryTests
    {
        [Test]
        public void Instantiate_NoConstructorParameters_Success()
        {
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection => serviceCollection.AddSingletonService<IServiceA, ServiceA>())
                .Build();

            provider.Initialize();

            var instance = provider.Resolve<IServiceA>();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ServiceA>(instance);
        }

        [Test]
        public void Instantiate_WithDefaultParameters_Success()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddStaticInstance<IServiceA>(serviceA)
                        .AddSingletonService<IServiceF, ServiceF>())
                .Build();

            provider.Initialize();

            var instance = provider.Resolve<IServiceF>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ServiceF>(instance);
            Assert.AreEqual(serviceA, instance.ServiceA);
        }

        [Test]
        public void Instantiate_PrefersNamedParameter_Success()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddStaticInstance<IServiceA>("serviceA", serviceA)
                        .AddSingletonService<IServiceA, ServiceA>()
                        .AddSingletonService<IServiceF, ServiceF>())
                .Build();

            provider.Initialize();

            var instance = provider.Resolve<IServiceF>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ServiceF>(instance);
            Assert.AreEqual(serviceA, instance.ServiceA);
        }

        [Test]
        public void Instantiate_ServiceMissing_InvalidDependencyDefinitionException()
        {
            var builder = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceF, ServiceF>());

            Assert.Throws<InvalidDependencyDefinitionException>(() => builder.Build());
        }

        [Test]
        public void BindParameterToValue_ValidParameter_Success()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory => 
                            serviceFactory.BindParameterToValue("serviceA", serviceA)))
                .Build();

            provider.Initialize();

            var instance = provider.Resolve<IServiceF>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ServiceF>(instance);
            Assert.AreEqual(serviceA, instance.ServiceA);
        }

        [Test]
        public void BindParameterToValue_InvalidParameterName_ArgumentException()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToValue("serviceB", serviceA)));

            Assert.Throws<ArgumentException>(() => provider.Build());
        }

        [Test]
        public void BindParameterToValue_InvalidParameterType_ArgumentException()
        {
            var serviceB = new ServiceB();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToValue("serviceA", serviceB)));

            Assert.Throws<ArgumentException>(() => provider.Build());
        }

        [Test]
        public void BindParameterToDelegate_ValidParameter_Success()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToDelegate<IServiceA>("serviceA",_ => serviceA)))
                .Build();

            provider.Initialize();

            var instance = provider.Resolve<IServiceF>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ServiceF>(instance);
            Assert.AreEqual(serviceA, instance.ServiceA);
        }

        [Test]
        public void BindParameterToDelegate_InvalidParameterName_ArgumentException()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToDelegate("serviceB", _ => serviceA)));

            Assert.Throws<ArgumentException>(() => provider.Build());
        }

        [Test]
        public void BindParameterToDelegate_InvalidParameterType_ArgumentException()
        {
            var serviceB = new ServiceB();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToDelegate("serviceA", _ => serviceB)));

            Assert.Throws<ArgumentException>(() => provider.Build());
        }

        [Test]
        public void BindParameterToService_ValidParameter_Success()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddStaticInstance<IServiceA>("myNamedService", serviceA)
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToService<IServiceA>("serviceA", "myNamedService")))
                .Build();

            provider.Initialize();

            var instance = provider.Resolve<IServiceF>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ServiceF>(instance);
            Assert.AreEqual(serviceA, instance.ServiceA);
        }

        [Test]
        public void BindParameterToService_InvalidParameterName_ArgumentException()
        {
            var serviceA = new ServiceA();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceA, ServiceA>()
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToService<IServiceA>("serviceB")));

            Assert.Throws<ArgumentException>(() => provider.Build());
        }

        [Test]
        public void BindParameterToService_InvalidParameterType_ArgumentException()
        {
            var serviceB = new ServiceB();
            var provider = new ServiceProviderBuilder()
                .Register(serviceCollection =>
                    serviceCollection
                        .AddSingletonService<IServiceB, ServiceB>()
                        .AddSingletonService<IServiceF, ServiceF>(serviceFactory =>
                            serviceFactory.BindParameterToService<IServiceB>("serviceA")));

            Assert.Throws<ArgumentException>(() => provider.Build());
        }
    }
}
