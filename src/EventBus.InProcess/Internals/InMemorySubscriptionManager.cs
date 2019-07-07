using System;
using System.Collections.Generic;
using System.Linq;

namespace EventBus.InProcess.Internals
{
    public sealed class InMemorySubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<Type, List<Type>> _handlers;

        public event EventHandler<EventRemovedArgs> OnEventRemoved;

        public InMemorySubscriptionManager()
        {
            _handlers = new Dictionary<Type, List<Type>>();
        }

        public void AddSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventType = typeof(T);
            var handlerType = typeof(TH);
            if (!HasSubscriptionsForEvent(eventType))
            {
                _handlers.Add(eventType, new List<Type>());
            }

            if (_handlers[eventType].Any(h => h == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventType.Name}'", nameof(handlerType));
            }

            _handlers[eventType].Add(handlerType);
        }

        public bool IsEmpty => _handlers.Keys.Count == 0;

        public void Clear()
        {
            _handlers.Clear();
        }

        public void RemoveSubscription<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var subsToRemove = typeof(TH);
            var eventType = typeof(T);
            if (!HasSubscription(eventType, subsToRemove))
            {
                return;
            }

            _handlers[eventType].Remove(subsToRemove);

            if (_handlers[eventType].Count == 0)
            {
                _handlers.Remove(eventType);

                RaiseOnEventRemoved(new EventRemovedArgs(eventType));
            }
        }

        public bool HasSubscriptionsForEvent(Type eventType) => _handlers.ContainsKey(eventType);

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent => _handlers.ContainsKey(typeof(T));

        public IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent =>
            HasSubscriptionsForEvent<T>()
                ? _handlers[typeof(T)]
                : Enumerable.Empty<Type>();

        private void RaiseOnEventRemoved(EventRemovedArgs args)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, args);
        }

        private bool HasSubscription(Type eventType, Type subType)
        {
            return HasSubscriptionsForEvent(eventType) && _handlers[eventType].Contains(subType);
        }
    }
}