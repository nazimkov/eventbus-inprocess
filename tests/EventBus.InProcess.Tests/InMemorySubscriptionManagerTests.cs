using EventBus.InProcess.Internals;
using EventBus.InProcess.Tests.Data;
using System;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class InMemorySubscriptionManagerTests
    {
        private readonly InMemorySubscriptionManager _manager;

        public InMemorySubscriptionManagerTests()
        {
            _manager = new InMemorySubscriptionManager();
        }

        [Fact]
        public void IsEmpty_NewManagerInstance_HandlersIsEmpty()
        {
            // Act
            // Assert
            Assert.True(_manager.IsEmpty);
        }

        [Fact]
        public void IsEmpty_OneManagerAdded_HandlersIsNotEmpty()
        {
            // Act
            _manager.AddSubscription<TestEvent, TestEventHandler>();

            // Assert
            Assert.False(_manager.IsEmpty);
        }

        [Fact]
        public void Clear_ManagerWithSubs_ClearSubs()
        {
            // Arrange
            _manager.AddSubscription<TestEvent, TestEventHandler>();

            // Act
            _manager.Clear();

            // Assert
            Assert.True(_manager.IsEmpty);
        }

        [Fact]
        public void HasSubscriptionsForEvent_ManagerWithEventSub_ReturnsTrue()
        {
            // Act
            _manager.AddSubscription<TestEvent, TestEventHandler>();

            // Assert
            Assert.True(_manager.HasSubscriptionsForEvent<TestEvent>());
            Assert.True(_manager.HasSubscriptionsForEvent(typeof(TestEvent)));
        }

        [Fact]
        public void HasSubscriptionsForEvent_NewManagerInstance_ReturnsFalse()
        {
            // Act
            // Assert
            Assert.False(_manager.HasSubscriptionsForEvent<TestEvent>());
            Assert.False(_manager.HasSubscriptionsForEvent(typeof(TestEvent)));
        }

        [Fact]
        public void GetHandlersForEvent_MultipleHandlers_ReturnsAllHandlers()
        {
            // Arrange
            _manager.AddSubscription<TestEvent, TestEventHandler>();
            _manager.AddSubscription<TestEvent, SecondTestEventHandler>();

            var expectedTypes = new Type[]
            {
                typeof(TestEventHandler),
                typeof(SecondTestEventHandler)
            };

            // Act
            var handlers = _manager.GetHandlersForEvent<TestEvent>();

            // Assert
            Assert.Equal(expectedTypes, handlers);
        }

        [Fact]
        public void GetHandlersForEvent_NewManagerInstance_ReturnsEmpty()
        {
            // Act
            var handlers = _manager.GetHandlersForEvent<TestEvent>();

            // Assert
            Assert.Empty(handlers);
        }

        [Fact]
        public void AddSubscription_NewSubForEvent_SubAdded()
        {
            // Act
            _manager.AddSubscription<TestEvent, TestEventHandler>();
            var handlers = _manager.GetHandlersForEvent<TestEvent>();

            // Assert
            Assert.Contains(typeof(TestEventHandler), handlers);
        }

        [Fact]
        public void AddSubscription_ExistingEventWithOnesub_SubAdded()
        {
            // Arrange
            _manager.AddSubscription<TestEvent, TestEventHandler>();

            // Act
            _manager.AddSubscription<TestEvent, SecondTestEventHandler>();
            var handlers = _manager.GetHandlersForEvent<TestEvent>();

            // Assert
            Assert.Contains(typeof(SecondTestEventHandler), handlers);
        }

        [Fact]
        public void AddSubscription_ExistingSubForEvent_ThrowsArgumentException()
        {
            // Arrange
            _manager.AddSubscription<TestEvent, TestEventHandler>();

            // Act
            // Assert
            Assert.Throws<ArgumentException>(() =>
                _manager.AddSubscription<TestEvent, TestEventHandler>());
        }

        [Fact]
        public void RemoveSubscription_ExistingEventWithOneSub_SubRemoved()
        {
            // Arrange
            _manager.AddSubscription<TestEvent, TestEventHandler>();

            // Act
            _manager.RemoveSubscription<TestEvent, TestEventHandler>();
            var handlers = _manager.GetHandlersForEvent<TestEvent>();

            // Assert
            Assert.Empty(handlers);
        }

        [Fact]
        public void RemoveSubscription_ExistingEventWithOneSub_RiseOnEventRemoved()
        {
            // Arrange
            _manager.AddSubscription<TestEvent, TestEventHandler>();

            // Act
            var evt = Assert.Raises<EventRemovedArgs>(
                h => _manager.OnEventRemoved += h,
                h => _manager.OnEventRemoved -= h,
                () => _manager.RemoveSubscription<TestEvent, TestEventHandler>());

            // Assert
            Assert.NotNull(evt);
            Assert.Equal(_manager, evt.Sender);
            Assert.Equal(typeof(TestEvent), evt.Arguments.Event);
        }

        [Fact]
        public void RemoveSubscription_ExistingEventWithMultipleSub_SubRemoved()
        {
            // Arrange
            _manager.AddSubscription<TestEvent, TestEventHandler>();
            _manager.AddSubscription<TestEvent, SecondTestEventHandler>();

            // Act
            _manager.RemoveSubscription<TestEvent, TestEventHandler>();
            var handlers = _manager.GetHandlersForEvent<TestEvent>();

            // Assert
            Assert.Contains(typeof(SecondTestEventHandler), handlers);
        }

        [Fact]
        public void RemoveSubscription_ManagerWithoutSubsForEvent_DoesNotThrowsException()
        {
            // Act
            // Assert
            _manager.RemoveSubscription<TestEvent, SecondTestEventHandler>();
        }
    }
}