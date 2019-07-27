using EventBus.InProcess.Tests.Common.Data;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests.Integration.Common
{
    public abstract class EventBusInProcessIntegrationTests
    {
        protected abstract void AddHandlerBuilder(Type handlerType, HandlerBuilder handlerBuilder = null);

        protected async Task Test_PublishAsync_EventWithHandler_HandlerReceivesEvent(Func<IEventBus> busBuilder)
        {
            // Arrange
            TestEvent receivedEvent = null;
            var sendedEvent = new TestEvent();
            var pause = new ManualResetEvent(false);
            AddHandlerBuilder(
                typeof(TestEventHandler),
                () => new TestEventHandler(e => { receivedEvent = e; pause.Set(); }));

            // Act
            var bus = busBuilder();
            bus.Subscribe<TestEvent, TestEventHandler>();
            await bus.PublishAsync(sendedEvent);

            // Assert
            AssertEventReceived(pause);
            Assert.NotNull(receivedEvent);
            AssertEventsEqual(sendedEvent, receivedEvent);
        }

        protected async Task Test_PublishAsync_EventWithMultipleHandlers_AllHandlersReceiveEvent(Func<IEventBus> busBuilder)
        {
            // Arrange
            TestEvent receivedEvent = null;
            var sendedEvent = new TestEvent();
            var (pauseFirst, pauseSecond) = (new ManualResetEvent(false), new ManualResetEvent(false));
            AddHandlerBuilder(
                typeof(TestEventHandler),
                () => new TestEventHandler(e => { receivedEvent = e; pauseFirst.Set(); }));
            AddHandlerBuilder(
                typeof(SecondTestEventHandler),
                () => new SecondTestEventHandler(e => { receivedEvent = e; pauseSecond.Set(); }));

            // Act
            var bus = busBuilder();
            bus.Subscribe<TestEvent, TestEventHandler>();
            bus.Subscribe<TestEvent, SecondTestEventHandler>();
            await bus.PublishAsync(sendedEvent);

            // Assert
            AssertEventReceived(pauseFirst);
            AssertEventReceived(pauseSecond);
            Assert.NotNull(receivedEvent);
            AssertEventsEqual(sendedEvent, receivedEvent);
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
    }
}