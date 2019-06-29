using EventBus.InProcess.Internals;
using EventBus.InProcess.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class EventBusInProcessTests
    {

        [Fact]
        public async Task PublishAsync_MessageWithSubs_HandlerGetsMessage()
        {
            // Arrange
            TestEvent receivedEvent = null;
            var sendedEvent = new TestEvent();
            var handler = new TestEventHandler(e => receivedEvent = e);

            var services = new Dictionary<Type, object>
            {
                { typeof(TestEventHandler), handler }
            };

            var scopeFactory = ServiceScopeFactoryMock.GetMock(services);
            var bus = new EventBusInProcess(null, null, scopeFactory.Object);

            // Act
            bus.Subscribe<TestEvent, TestEventHandler>();
            await bus.PublishAsync(sendedEvent);

            // Assert
            Assert.NotNull(receivedEvent);
            AssertEventsEqual(sendedEvent, receivedEvent);

        }

        private void AssertEventsEqual(IntegrationEvent expected, IntegrationEvent given)
        {
            Assert.Equal(expected.Id, given.Id);
            Assert.Equal(expected.CreationDate, given.CreationDate);
        }

        internal class TestEvent : IntegrationEvent
        {
        }

        internal class TestEventHandler : IIntegrationEventHandler<TestEvent>
        {
            private readonly Action<TestEvent> _callback;

            public TestEventHandler(Action<TestEvent> callback)
            {
                _callback = callback;
            }
            public Task Handle(TestEvent @event)
            {
                _callback.Invoke(@event);
                return Task.CompletedTask;
            }
        }
    }
}
