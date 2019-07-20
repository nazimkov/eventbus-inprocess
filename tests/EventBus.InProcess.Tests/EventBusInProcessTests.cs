using EventBus.InProcess.Internals.Channels;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class EventBusInProcessTests
    {
        private readonly Mock<IEventBusSubscriptionManager> _subManagerMock;
        private readonly Mock<IChanneslManager> _channelsManagerMock;
        private readonly Mock<IEventProcessor> _eventProcessorMock;
        private readonly Mock<IHandlerProvider> _handlerProviderMock;

        public EventBusInProcessTests()
        {
            _subManagerMock = new Mock<IEventBusSubscriptionManager>();
            _channelsManagerMock = new Mock<IChanneslManager>();
            _eventProcessorMock = new Mock<IEventProcessor>();
            _handlerProviderMock = new Mock<IHandlerProvider>();
        }

        [Theory]
        [MemberData(nameof(GetCtorExceptionTestData))]
        public void Ctor_OneOfParameterIsNull_ThrowsArgumentNullException(
            IEventBusSubscriptionManager subsManager,
            IChanneslManager channelManager,
            IEventProcessor eventProcessor,
            IHandlerProvider handlerProvider)
        {
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new ProtectedExposedEventBus(subsManager, channelManager, eventProcessor, handlerProvider));
        }

        [Fact]
        public void Ctor_AllParametersPassed_InstanceCreated()
        {
            // Act
            var bus = new ProtectedExposedEventBus(_subManagerMock.Object, _channelsManagerMock.Object, _eventProcessorMock.Object, _handlerProviderMock.Object);

            // Assert
            Assert.NotNull(bus);
        }

        public static IEnumerable<object[]> GetCtorExceptionTestData()
        {
            var subManagerEmpty = new Mock<IEventBusSubscriptionManager>().Object;
            var channelManagerEmpty = new Mock<IChanneslManager>().Object;
            var eventProcessorEmpty = new Mock<IEventProcessor>().Object;
            var scopeFactoryEmpty = new Mock<IHandlerProvider>().Object;

            yield return new object[] { null, channelManagerEmpty, eventProcessorEmpty, scopeFactoryEmpty };
            yield return new object[] { subManagerEmpty, null, eventProcessorEmpty, scopeFactoryEmpty };
            yield return new object[] { subManagerEmpty, channelManagerEmpty, null, scopeFactoryEmpty };
            yield return new object[] { subManagerEmpty, channelManagerEmpty, eventProcessorEmpty, null };
        }

        public class ProtectedExposedEventBus : EventBusInProcess
        {
            public ProtectedExposedEventBus(
                IEventBusSubscriptionManager subsManager,
                IChanneslManager channelManager,
                IEventProcessor eventProcessor,
                IHandlerProvider handlerProvider)
                : base(subsManager, channelManager, eventProcessor, handlerProvider)
            {
            }
        }
    }
}