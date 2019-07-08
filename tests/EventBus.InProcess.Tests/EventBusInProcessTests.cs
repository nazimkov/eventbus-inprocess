using EventBus.InProcess.Internals;
using EventBus.InProcess.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class EventBusInProcessTests
    {
        private readonly Mock<IEventBusSubscriptionManager> _subManagerMock;
        private readonly Mock<IChannelManager> _channelManagerMock;
        private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;

        public EventBusInProcessTests()
        {
            _subManagerMock = new Mock<IEventBusSubscriptionManager>();
            _channelManagerMock = new Mock<IChannelManager>();
            _scopeFactoryMock = ServiceScopeFactoryMock.GetMock(new Dictionary<Type, object>());
        }

        [Theory]
        [MemberData(nameof(GetCtorExceptionTestData))]
        public void Ctor_OneOfParameterIsNull_ThrowsArgumentNullException(
            IEventBusSubscriptionManager subsManager,
            IChannelManager channelManager,
            IServiceScopeFactory scopeFactory)
        {
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new EventBusInProcess(subsManager, channelManager, scopeFactory));
        }

        [Fact]
        public void Ctor_AllParametersPassed_InstanceCreated()
        {
            // Act
            var bus = new EventBusInProcess(_subManagerMock.Object, _channelManagerMock.Object, _scopeFactoryMock.Object);

            // Assert
            Assert.NotNull(bus);
        }

        public static IEnumerable<object[]> GetCtorExceptionTestData()
        {
            var subManagerEmpty = new Mock<IEventBusSubscriptionManager>().Object;
            var channelManagerEmpty = new Mock<IChannelManager>().Object;
            var scopeFactoryEmpty = new Mock<IServiceScopeFactory>().Object;

            yield return new object[] { null, channelManagerEmpty, scopeFactoryEmpty };
            yield return new object[] { subManagerEmpty, null, scopeFactoryEmpty };
            yield return new object[] { subManagerEmpty, channelManagerEmpty, null };
        }
    }
}