using EventBus.InProcess.Internals;
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
        private readonly IHandlerProvider _handlerProvider;
        private readonly CancellationTokenSource _cts;

        protected EventBusInProcess(
            IEventBusSubscriptionManager subsManager,
            IChanneslManager channelManager,
            IHandlerProvider handlerProvider)
        {
            _subsManager = subsManager ??
                throw new ArgumentNullException(nameof(subsManager));
            _channelManager = channelManager ??
                throw new ArgumentNullException(nameof(subsManager));
            _handlerProvider = handlerProvider ??
                throw new ArgumentNullException(nameof(handlerProvider));
            _cts = new CancellationTokenSource();
        }

        public EventBusInProcess(IHandlerProvider handlerProvider)
        {
            _subsManager = new InMemorySubscriptionManager();
            _channelManager = new ThreadChanelsManager();
            _handlerProvider = handlerProvider ??
                throw new ArgumentNullException(nameof(handlerProvider));
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
                foreach (var handlerType in _subsManager.GetHandlersForEvent<T>())
                {
                    var handler = (IntegrationEventHandlerWrapper<T>)Activator.CreateInstance(typeof(IntegrationEventHandlerWrapperImpl<,>).MakeGenericType(typeof(T), handlerType));
                    await handler.HandleAsync(@event, _handlerProvider, _cts.Token);
                }
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