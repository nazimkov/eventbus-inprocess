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
        private readonly Mock<IHandlerProvider> _handlerProviderMock;

        public EventBusInProcessTests()
        {
            _subManagerMock = new Mock<IEventBusSubscriptionManager>();
            _channelsManagerMock = new Mock<IChanneslManager>();
            _handlerProviderMock = new Mock<IHandlerProvider>();
        }

        [Theory]
        [MemberData(nameof(GetCtorExceptionTestData))]
        public void Ctor_OneOfParameterIsNull_ThrowsArgumentNullException(
            IEventBusSubscriptionManager subsManager,
            IChanneslManager channelManager,
            IHandlerProvider handlerProvider)
        {
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new ProtectedExposedEventBus(subsManager, channelManager, handlerProvider));
        }

        [Fact]
        public void Ctor_AllParametersPassed_InstanceCreated()
        {
            // Act
            var bus = new ProtectedExposedEventBus(_subManagerMock.Object, _channelsManagerMock.Object, _handlerProviderMock.Object);

            // Assert
            Assert.NotNull(bus);
        }

        public static IEnumerable<object[]> GetCtorExceptionTestData()
        {
            var subManagerEmpty = new Mock<IEventBusSubscriptionManager>().Object;
            var channelManagerEmpty = new Mock<IChanneslManager>().Object;
            var handlerProviderEmpty = new Mock<IHandlerProvider>().Object;

            yield return new object[] { null, channelManagerEmpty, handlerProviderEmpty };
            yield return new object[] { subManagerEmpty, null, handlerProviderEmpty };
            yield return new object[] { subManagerEmpty, channelManagerEmpty, null };
        }

        public class ProtectedExposedEventBus : EventBusInProcess
        {
            public ProtectedExposedEventBus(
                IEventBusSubscriptionManager subsManager,
                IChanneslManager channelManager,
                IHandlerProvider handlerProvider)
                : base(subsManager, channelManager, handlerProvider)
            {
            }
        }
    }
}