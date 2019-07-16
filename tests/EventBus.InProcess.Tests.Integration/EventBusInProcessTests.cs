using EventBus.InProcess.Tests.Common.Data;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests.Integration
{
    public class EventBusInProcessTests
    {
        private readonly EventBusInProcess _bus;

        public EventBusInProcessTests()
        {
            _bus = new EventBusInProcess();
        }

        [Fact]
        public async Task PublishAsync_EventWithHandler_HandlerReceivesEvent()
        {
            // Arrange
            TestEvent receivedEvent = null;
            var sendedEvent = new TestEvent();
            var pause = new ManualResetEvent(false);
            _bus.Subscribe<TestEvent, TestEventHandler>(() => new TestEventHandler(e => { receivedEvent = e; pause.Set(); }));


            // Act
            await _bus.PublishAsync(sendedEvent);

            // Assert
            AssertEventReceived(pause);
            Assert.NotNull(receivedEvent);
            AssertEventsEqual(sendedEvent, receivedEvent);
        }

        [Fact]
        public async Task PublishAsync_EventWithMultipleHandlers_AllHandlersReceiveEvent()
        {
            // Arrange
            TestEvent receivedEvent = null;
            var sendedEvent = new TestEvent();
            var (pauseFirst, pauseSecond) = (new ManualResetEvent(false), new ManualResetEvent(false));
            _bus.Subscribe<TestEvent, TestEventHandler>(() => new TestEventHandler(e => { receivedEvent = e; pauseFirst.Set(); }));
            _bus.Subscribe<TestEvent, SecondTestEventHandler>(() => new SecondTestEventHandler(e => { receivedEvent = e; pauseSecond.Set(); }));

            // Act
            await _bus.PublishAsync(sendedEvent);

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