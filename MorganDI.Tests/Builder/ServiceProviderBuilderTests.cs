using Moq;
using MorganDI.Builder;
using MorganDI.Tests.Mocks;
using NUnit.Framework;
using System;

namespace MorganDI.Tests.Builder
{
    public class ServiceProviderBuilderTests
    {
        private readonly Mock<ServiceProviderBuilderEventHandler>  _buildStartedHandlerMock = new Mock<ServiceProviderBuilderEventHandler>();
        private readonly Mock<ServiceProviderBuilderEventHandler>  _buildCompletedHandlerMock = new Mock<ServiceProviderBuilderEventHandler>();

        private Mock<ServiceConfigurationDelegate> CreateConfigurationDelegateMock()
        {
            var configurationMock = new Mock<ServiceConfigurationDelegate>();
            configurationMock.Setup(x => x(It.IsAny<IServiceCollection>())).Verifiable();
            return configurationMock;
        }

        private void VerifyConfigurationDelegateMock(Mock<ServiceConfigurationDelegate> configurationMock)
        {
            configurationMock.Verify(x => x(It.IsAny<IServiceCollection>()), Times.Once());
        }

        private void SetupEventHandlersForBuilder(IServiceProviderBuilder builder, ServiceProviderBuilderEventHandler startedCallback = null)
        {
            if (startedCallback != null)
                _buildStartedHandlerMock.Setup(x => x(builder, It.IsNotNull<IServiceProvider>())).Callback(startedCallback).Verifiable();
            else
                _buildStartedHandlerMock.Setup(x => x(builder, It.IsNotNull<IServiceProvider>())).Verifiable();

            _buildCompletedHandlerMock.Setup(x => x(builder, It.IsNotNull<IServiceProvider>())).Verifiable();

            builder.ServiceProviderBuildStarted += _buildStartedHandlerMock.Object;
            builder.ServiceProviderBuildComplete += _buildCompletedHandlerMock.Object;
        }

        private void VerifyBuilderEventsFired(IServiceProviderBuilder builder)
        {
            _buildStartedHandlerMock.Verify(x => x(builder, It.IsNotNull<IServiceProvider>()), Times.Once());
            _buildCompletedHandlerMock.Verify(x => x(builder, It.IsNotNull<IServiceProvider>()), Times.Once());
        }

        [Test]
        public void RegisterServiceConfiguration_Null_ArgumentNullException()
        {
            var builder = new ServiceProviderBuilder();
            Assert.Throws<ArgumentNullException>(() => builder.Register(null));
        }

        [Test]
        public void Build_Empty_Success()
        {
            var builder = new ServiceProviderBuilder();
            SetupEventHandlersForBuilder(builder);
            var serviceProvider = builder.Build();
            VerifyBuilderEventsFired(builder);
            Assert.IsNotNull(serviceProvider);
        }

        [Test]
        public void Build_WithSingleConfiguration_Success()
        {
            var configurationMock = CreateConfigurationDelegateMock();
            var builder = new ServiceProviderBuilder();
            SetupEventHandlersForBuilder(builder);
            builder.Register(configurationMock.Object);
            var serviceProvider = builder.Build();
            VerifyConfigurationDelegateMock(configurationMock);
            VerifyBuilderEventsFired(builder);
            Assert.IsNotNull(serviceProvider);
        }

        [Test]
        public void Build_WithMultipleConfiguration_Success()
        {
            var firstConfigurationMock = CreateConfigurationDelegateMock();
            var secondConfigurationMock = CreateConfigurationDelegateMock();

            var builder = new ServiceProviderBuilder();
            SetupEventHandlersForBuilder(builder);
            builder
                .Register(firstConfigurationMock.Object)
                .Register(secondConfigurationMock.Object);
            var serviceProvider = builder.Build();
            VerifyConfigurationDelegateMock(firstConfigurationMock);
            VerifyConfigurationDelegateMock(secondConfigurationMock);
            VerifyBuilderEventsFired(builder);
            Assert.IsNotNull(serviceProvider);
        }

        [Test]
        public void Build_CircularDependency_InvalidDependencyDefinitionException()
        {
            var builder = new ServiceProviderBuilder();
            builder.Register(serviceCollection =>
                serviceCollection
                    .AddSingletonService<ServiceBC>()
                    .AddServiceAlias<IServiceB, ServiceBC>()
                    .AddServiceAlias<IServiceC, ServiceBC>()
                    .AddSingletonService<ServiceDE>()
                    .AddServiceAlias<IServiceD, ServiceDE>()
                    .AddServiceAlias<IServiceE, ServiceDE>());

            Assert.Throws<InvalidDependencyDefinitionException>(() => builder.Build());
        }
    }
}
