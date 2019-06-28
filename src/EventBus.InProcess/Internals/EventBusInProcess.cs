using System;

namespace EventBus.InProcess.Internals
{
    internal class EventBusInProcess : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subsManager;

        public EventBusInProcess(IEventBusConfig config, IEventBusSubscriptionManager subsManager)
        {
            _subsManager = subsManager ?? throw new ArgumentNullException(nameof(_subsManager));
        }

        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}