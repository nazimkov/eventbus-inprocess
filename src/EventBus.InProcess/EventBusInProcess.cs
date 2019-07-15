using EventBus.InProcess.Internals.Channels;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("EventBus.InProcess.Tests")]

namespace EventBus.InProcess
{
    public class EventBusInProcess : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subsManager;
        private readonly IChanneslManager _channelManager;
        private readonly IEventProcessor _eventProcessor;
        private readonly CancellationTokenSource _cts;

        public EventBusInProcess(
            IEventBusSubscriptionManager subsManager,
            IChanneslManager channelManager,
            IEventProcessor eventProcessor)
        {
            _subsManager = subsManager ??
                throw new ArgumentNullException(nameof(subsManager));
            _channelManager = channelManager ??
                throw new ArgumentNullException(nameof(subsManager));
            _eventProcessor = eventProcessor ??
                throw new ArgumentNullException(nameof(eventProcessor));
            _cts = new CancellationTokenSource();
        }

        public void Publish<T>(T @event) where T : IntegrationEvent
        {
            var channel = _channelManager.Get<T>();
            channel.WriteAsync(@event, CancellationToken.None).GetAwaiter().GetResult();
        }

        public async Task PublishAsync<T>(
            T @event,
            CancellationToken cancellationToken = default)
        where T : IntegrationEvent
        {
            var channel = _channelManager.Get<T>();
            await channel.WriteAsync(@event, cancellationToken).ConfigureAwait(false);
        }

        public void Subscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
        {
            _subsManager.AddSubscription<T, TH>();
            _channelManager.CreateAsync<T>(ProcessEvent, _cts.Token).GetAwaiter().GetResult();
        }

        public void Unsubscribe<T, TH>()
        where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
        {
            _subsManager.RemoveSubscription<T, TH>();
        }

        private async ValueTask ProcessEvent<T>(T @event) where T : IntegrationEvent
        {
            if (_subsManager.HasSubscriptionsForEvent<T>())
            {
                var handlersTypes = _subsManager.GetHandlersForEvent<T>();
                await _eventProcessor.ProcessEventAsync(@event, handlersTypes, _cts.Token);
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts.Cancel();
                    _channelManager.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}