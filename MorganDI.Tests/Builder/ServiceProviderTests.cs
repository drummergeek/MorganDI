using Moq;
using MorganDI.Builder;
using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace MorganDI.Tests.Builder
{
    public class ServiceProviderTests
    {
        private const string SERVICE_NAME = "service";

        private ServiceConfigurationDelegate GetDefaultConfigurationDelegate(string name = null, Scope scope = Scope.Singleton) =>
            (serviceCollection) => serviceCollection.AddServiceToScope<IServiceA, ServiceA>(scope, name);

        private Mock<ServiceRequestedEventHandler> GetRequestedEventHandlerMock(ServiceIdentifier identifier)
        {
            var mock = new Mock<ServiceRequestedEventHandler>();
            mock.Setup(x => x(It.IsAny<IServiceProvider>(), identifier)).Verifiable();
            return mock;
        }

        private void VerifyRequestedEventHandler(Mock<ServiceRequestedEventHandler> mock, ServiceIdentifier identifier, Times times)
        {
            mock.Verify(x => x(It.IsAny<IServiceProvider>(), identifier), times);
            mock.Reset();
        }

        private Mock<ServiceResolvedEventHandler> GetResolvedEventHandlerMock<TService>(ServiceIdentifier identifier)
        {
            var mock = new Mock<ServiceResolvedEventHandler>();
            mock.Setup(x => x(It.IsAny<IServiceProvider>(), identifier, It.IsAny<TService>())).Verifiable();
            return mock;
        }

        private void VerifyResolvedEventHandler<TService>(Mock<ServiceResolvedEventHandler> mock, ServiceIdentifier identifier, Times times)
        {
            mock.Verify(x => x(It.IsAny<IServiceProvider>(), identifier, It.IsAny<TService>()), times);
            mock.Reset();
        }

        private IServiceProvider GetServiceProvider(
            ServiceConfigurationDelegate configurationDelegate = null,
            ServiceProviderEventHandler initializeRequestedHandler = null,
            ServiceProviderEventHandler initializeCompletedHandler = null,
            ServiceProviderEventHandler sceneTeardownRequestedHandler = null,
            ServiceProviderEventHandler sceneTeardownCompletedHandler = null,
            ServiceRequestedEventHandler serviceRequestedHandler = null,
            ServiceResolvedEventHandler serviceInstantiatedHandler = null,
            ServiceResolvedEventHandler serviceResolvedHandler = null
            )
        {
            var builder = new ServiceProviderBuilder()
                .Register(configurationDelegate ?? GetDefaultConfigurationDelegate());

            builder.ServiceProviderBuildStarted +=
                (builder, provider) =>
                {
                    if (initializeRequestedHandler != null)
                        provider.InitializeRequested += initializeRequestedHandler;
                    if (initializeCompletedHandler != null)
                        provider.InitializeComplete += initializeCompletedHandler;
                    if (sceneTeardownRequestedHandler != null)
                        provider.SceneTeardownRequested += sceneTeardownRequestedHandler;
                    if (sceneTeardownCompletedHandler != null)
                        provider.SceneTeardownComplete += sceneTeardownCompletedHandler;
                    if (serviceRequestedHandler != null)
                        provider.ServiceRequested += serviceRequestedHandler;
                    if (serviceInstantiatedHandler != null)
                        provider.ServiceInstantiated += serviceInstantiatedHandler;
                    if (serviceResolvedHandler != null)
                        provider.ServiceResolved += serviceResolvedHandler;
                };

            return builder.Build();
        }

        [Test]
        public void ServiceExists_ServiceMissing_False()
        {
            var provider = GetServiceProvider(_ => { });
            Assert.IsFalse(provider.ServiceExists(ServiceIdentifier.Create<IServiceA>(), Scope.Singleton));
        }

        [Test]
        public void ServiceExists_AllValidScopeAndNamePermutations(
            [Values(null, SERVICE_NAME)] string serviceName,
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope serviceScope,
            [Values(Scope.Singleton, Scope.Scene, Scope.Transient)] Scope checkScope)
        {
            // Only when the check scope is at or higher than the service should it return true.
            bool expectedResult = checkScope >= serviceScope;

            var provider = GetServiceProvider(GetDefaultConfigurationDelegate(serviceName, serviceScope));

            Assert.AreEqual(expectedResult, provider.ServiceExists(ServiceIdentifier.Create<IServiceA>(serviceName), checkScope));
        }

        [Test]
        public void ServiceExists_ExistingService_DoesNotFireRequestedEvent()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var provider = GetServiceProvider(serviceRequestedHandler: requestedHandler.Object);
            _ = provider.ServiceExists(identifier, Scope.Singleton);
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Never());
        }

        [Test]
        public void Initialize_InstantiatesSingletons_Success()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var resolvedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var instantiatedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var initializeRequestedHandler = new Mock<ServiceProviderEventHandler>();
            var initializeCompletedHandler = new Mock<ServiceProviderEventHandler>();

            initializeRequestedHandler
                .Setup(x => x(It.IsAny<IServiceProvider>()))
                // Check to make sure we are called first.
                .Callback(() => initializeCompletedHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Never()))
                .Verifiable();

            initializeCompletedHandler.Setup(x => x(It.IsAny<IServiceProvider>())).Verifiable();

            var provider = GetServiceProvider(
                initializeRequestedHandler: initializeRequestedHandler.Object,
                initializeCompletedHandler: initializeCompletedHandler.Object,
                serviceRequestedHandler: requestedHandler.Object,
                serviceInstantiatedHandler: instantiatedHandler.Object,
                serviceResolvedHandler: resolvedHandler.Object);

            // Ensure they don't fire until initialize is requested
            initializeRequestedHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Never());
            initializeCompletedHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Never());

            provider.Initialize();

            initializeRequestedHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Once());
            initializeCompletedHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Once());

            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Once());
        }

        [Test]
        public void Resolve_ExistingService_Success()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var resolvedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var instantiatedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);

            var provider = GetServiceProvider(
                serviceRequestedHandler: requestedHandler.Object,
                serviceInstantiatedHandler: instantiatedHandler.Object,
                serviceResolvedHandler: resolvedHandler.Object);

            provider.Initialize();

            // Reset handlers to remove invocations during initialization
            requestedHandler.Reset();
            instantiatedHandler.Reset();
            resolvedHandler.Reset();

            var instance = provider.Resolve(identifier);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOf<ServiceA>(instance);

            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Once());
        }

        [Test]
        public void Resolve_SingletonInstantiatesOnlyOnce_Success()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var resolvedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var instantiatedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);

            var provider = GetServiceProvider(
                serviceRequestedHandler: requestedHandler.Object,
                serviceInstantiatedHandler: instantiatedHandler.Object,
                serviceResolvedHandler: resolvedHandler.Object);

            provider.Initialize();

            // Reset handlers to remove invocations during initialization
            requestedHandler.Reset();
            instantiatedHandler.Reset();
            resolvedHandler.Reset();

            var instance1 = provider.Resolve(identifier);
            var instance2 = provider.Resolve(identifier);

            Assert.AreEqual(instance1, instance2);

            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Exactly(2));
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Exactly(2));
        }

        [Test]
        public void Resolve_SceneOnlyInstantiatesOncePerScene_Success()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var resolvedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var instantiatedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);

            var provider = GetServiceProvider(
                configurationDelegate: GetDefaultConfigurationDelegate(scope: Scope.Scene),
                serviceRequestedHandler: requestedHandler.Object,
                serviceInstantiatedHandler: instantiatedHandler.Object,
                serviceResolvedHandler: resolvedHandler.Object);

            provider.Initialize();

            // Ensure the scene service was not instantiated during singleton initialization
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Never());

            var instance1 = provider.Resolve(identifier);
            var instance2 = provider.Resolve(identifier);

            Assert.AreEqual(instance1, instance2);
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Exactly(2));
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Exactly(2));
            
            // Reset the scene
            provider.TeardownScene();

            var instance3 = provider.Resolve(identifier);

            Assert.AreNotEqual(instance2, instance3);

            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Once());
        }

        [Test]
        public void Resolve_TransientInstantiateEveryRequest_Success()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var resolvedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var instantiatedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);

            var provider = GetServiceProvider(
                configurationDelegate: GetDefaultConfigurationDelegate(scope: Scope.Transient),
                serviceRequestedHandler: requestedHandler.Object,
                serviceInstantiatedHandler: instantiatedHandler.Object,
                serviceResolvedHandler: resolvedHandler.Object);

            provider.Initialize();

            // Ensure the scene service was not instantiated during singleton initialization
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Never());

            var instance1 = provider.Resolve(identifier);
            var instance2 = provider.Resolve(identifier);

            Assert.AreNotEqual(instance1, instance2);
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Exactly(2));
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Exactly(2));
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Exactly(2));
        }

        [Test]
        public void Resolve_StaticInstance_Success()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var instance = new ServiceA();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var resolvedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var instantiatedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);

            var provider = GetServiceProvider(
                configurationDelegate: serviceCollection => serviceCollection.AddStaticInstance<IServiceA>(instance),
                serviceRequestedHandler: requestedHandler.Object,
                serviceInstantiatedHandler: instantiatedHandler.Object,
                serviceResolvedHandler: resolvedHandler.Object);

            provider.Initialize();
            
            // Statics are always singleton, instantiation is called during initialization on all singletons, regardless of registration type
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Once());

            var resolvedInstance = provider.Resolve(identifier);

            Assert.AreEqual(instance, resolvedInstance);
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Once());
        }

        [Test]
        public void Resolve_DelegateInstance_Success()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();
            var instance = new ServiceA();
            var requestedHandler = GetRequestedEventHandlerMock(identifier);
            var resolvedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);
            var instantiatedHandler = GetResolvedEventHandlerMock<ServiceA>(identifier);

            var provider = GetServiceProvider(
                configurationDelegate: serviceCollection => serviceCollection.AddSingletonServiceDelegate<IServiceA>(_ => instance),
                serviceRequestedHandler: requestedHandler.Object,
                serviceInstantiatedHandler: instantiatedHandler.Object,
                serviceResolvedHandler: resolvedHandler.Object);

            provider.Initialize();

            // Statics are always singleton, instantiation is called during initialization on all singletons, regardless of registration type
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Once());

            var resolvedInstance = provider.Resolve(identifier);

            Assert.AreEqual(instance, resolvedInstance);
            VerifyRequestedEventHandler(requestedHandler, identifier, Times.Once());
            VerifyResolvedEventHandler<ServiceA>(instantiatedHandler, identifier, Times.Never());
            VerifyResolvedEventHandler<ServiceA>(resolvedHandler, identifier, Times.Once());
        }

        [Test]
        public void Resolve_CircularDependencyFromDelegates_InvalidDependencyDefinitionException()
        {
            var identifier = ServiceIdentifier.Create<IServiceA>();

            ServiceDelegate<IServiceA> serviceADelegate = x =>
            {
                x.Resolve<IServiceB>();
                return null;
            };
            ServiceDelegate<IServiceB> serviceBDelegate = x =>
            {
                x.Resolve<IServiceA>();
                return null;
            };

            var provider = GetServiceProvider(
                configurationDelegate: serviceCollection =>
                    serviceCollection
                        .AddSceneServiceDelegate<IServiceA>(serviceADelegate)
                        .AddSceneServiceDelegate<IServiceB>(serviceBDelegate));

            provider.Initialize();
            Assert.Throws<InvalidDependencyDefinitionException>(() => provider.Resolve(identifier));
        }

        [Test]
        public void TeardownScene_FiresEvents_Success()
        {
            var teardownRequestHandler = new Mock<ServiceProviderEventHandler>();
            var teardownCompletedHandler = new Mock<ServiceProviderEventHandler>();

            teardownRequestHandler
                .Setup(x => x(It.IsAny<IServiceProvider>()))
                // Check to make sure we are called first.
                .Callback(() => teardownCompletedHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Never()))
                .Verifiable();

            teardownCompletedHandler.Setup(x => x(It.IsAny<IServiceProvider>())).Verifiable();

            var provider = GetServiceProvider(
                sceneTeardownRequestedHandler: teardownRequestHandler.Object,
                sceneTeardownCompletedHandler: teardownCompletedHandler.Object);

            provider.Initialize();

            provider.TeardownScene();

            teardownRequestHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Once());
            teardownCompletedHandler.Verify(x => x(It.IsAny<IServiceProvider>()), Times.Once());
        }

        [Test]
        public void TeardownScene_DisposesIDisposable_Success()
        {
            var mockDisposable = new Mock<IDisposable>();
            mockDisposable.Setup(x => x.Dispose()).Verifiable();
            var identifier = ServiceIdentifier.Create<IDisposable>();
            var provider = GetServiceProvider(
                configurationDelegate: serviceCollection =>
                {
                    serviceCollection.AddSceneServiceDelegate<IDisposable>(_ => mockDisposable.Object);
                });

            provider.Initialize();
            _ = provider.Resolve(identifier);
            provider.TeardownScene();

            mockDisposable.Verify(x => x.Dispose(), Times.Once());
        }
    }
}
