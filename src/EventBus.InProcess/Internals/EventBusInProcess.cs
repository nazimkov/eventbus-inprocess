using EventBus.InProcess.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace EventBus.InProcess.Internals
{
    internal class EventBusInProcess : IEventBus
    {
        public EventBusInProcess(IEventBusConfig config)
        {
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
