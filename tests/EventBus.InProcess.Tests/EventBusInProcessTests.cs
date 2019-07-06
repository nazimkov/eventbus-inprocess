using EventBus.InProcess.Internals;
using EventBus.InProcess.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class EventBusInProcessTests
    {
        [Fact]
        public async Task PublishAsync_EventWithHandler_HandlerReceivesEvent()
        {
            // Arrange
            TestEvent receivedEvent = null;
            var sendedEvent = new TestEvent();
            var pause = new ManualResetEvent(false);
            var handler = new TestEventHandler(e => { receivedEvent = e; pause.Set(); });

            var services = new Dictionary<Type, object>
            {
                { typeof(TestEventHandler), handler }
            };

            var bus = BuildBus(services);

            // Act
            bus.Subscribe<TestEvent, TestEventHandler>();
            await bus.PublishAsync(sendedEvent);

            // Assert
            Assert.True(pause.WaitOne(100));
            Assert.NotNull(receivedEvent);
            AssertEventsEqual(sendedEvent, receivedEvent);
        }

        [Fact]
        public async Task PublishAsync_EventWithMultipleHandlers_AllHandlersReceiveEvent()
        {
            // Arrange
            TestEvent receivedEvent = null;
            var sendedEvent = new TestEvent();
            var pauseFirst = new ManualResetEvent(false);
            var pauseSecond = new ManualResetEvent(false);
            var handlerFirst = new TestEventHandler(e => { receivedEvent = e; pauseFirst.Set(); });
            var handlerSecond = new SecondTestEventHandler(e => { receivedEvent = e; pauseSecond.Set(); });

            var services = new Dictionary<Type, object>
            {
                { typeof(TestEventHandler), handlerFirst },
                { typeof(SecondTestEventHandler), handlerSecond }
            };

            var bus = BuildBus(services);

            // Act
            bus.Subscribe<TestEvent, TestEventHandler>();
            bus.Subscribe<TestEvent, SecondTestEventHandler>();
            await bus.PublishAsync(sendedEvent);

            // Assert
            AssertEventReceived(pauseFirst);
            AssertEventReceived(pauseSecond);
            Assert.NotNull(receivedEvent);
            AssertEventsEqual(sendedEvent, receivedEvent);
        }

        private static EventBusInProcess BuildBus(Dictionary<Type, object> services)
        {
            var scopeFactory = ServiceScopeFactoryMock.GetMock(services);
            var channelManager = new ChannelManager();
            var subsManager = new InMemorySubscriptionManager();
            return new EventBusInProcess(subsManager, channelManager, scopeFactory.Object);
        }

        private void AssertEventReceived(ManualResetEvent resetEvent)
        {
            Assert.True(resetEvent.WaitOne(100));
        }

        private void AssertEventsEqual(IntegrationEvent expected, IntegrationEvent given)
        {
            Assert.Equal(expected.Id, given.Id);
            Assert.Equal(expected.CreationDate, given.CreationDate);
        }

        internal class TestEvent : IntegrationEvent
        {
        }

        internal class SecondTestEventHandler : TestEventHandler
        {
            public SecondTestEventHandler(Action<TestEvent> callback) : base(callback)
            {
            }
        }

        internal class TestEventHandler : IIntegrationEventHandler<TestEvent>
        {
            private readonly Action<TestEvent> _callback;

            public TestEventHandler(Action<TestEvent> callback)
            {
                _callback = callback;
            }

            public Task HandleAsync(TestEvent @event, CancellationToken token)
            {
                _callback.Invoke(@event);
                return Task.CompletedTask;
            }
        }
    }
}