using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess
{
    public interface IEventBus : IDisposable
    {
        void Publish<T>(T @event)
            where T : IntegrationEvent;

        Task PublishAsync<T>(
            T @event,
            CancellationToken cancellationToken = default)
            where T : IntegrationEvent;

        void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        void Unsubscribe<T, TH>()
            where TH : IIntegrationEventHandler<T>
            where T : IntegrationEvent;
    }
}