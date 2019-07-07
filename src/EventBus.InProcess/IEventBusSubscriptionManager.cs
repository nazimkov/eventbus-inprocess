using System;
using System.Collections.Generic;

namespace EventBus.InProcess
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }

        event EventHandler<EventRemovedArgs> OnEventRemoved;

        void AddSubscription<T, TH>()
           where T : IntegrationEvent
           where TH : IIntegrationEventHandler<T>;

        void RemoveSubscription<T, TH>()
             where TH : IIntegrationEventHandler<T>
             where T : IntegrationEvent;

        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;

        IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent;

        void Clear();
    }
}