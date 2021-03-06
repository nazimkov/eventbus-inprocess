﻿using EventBus.InProcess.Internals.Channels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("EventBus.InProcess.Tests")]

namespace EventBus.InProcess.Internals
{
    internal class EventBusInProcess : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subsManager;
        private readonly IChanneslManager _channelManager;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CancellationTokenSource _cts;

        public EventBusInProcess(
            IEventBusSubscriptionManager subsManager,
            IChanneslManager channelManager,
            IServiceScopeFactory scopeFactory)
        {
            _subsManager = subsManager ??
                throw new ArgumentNullException(nameof(subsManager));
            _channelManager = channelManager ??
                throw new ArgumentNullException(nameof(subsManager));
            _scopeFactory = scopeFactory ??
                throw new ArgumentNullException(nameof(scopeFactory));
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

        public async ValueTask ProcessEvent<T>(T @event) where T : IntegrationEvent
        {
            if (_subsManager.HasSubscriptionsForEvent<T>())
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    foreach (var handlerType in _subsManager.GetHandlersForEvent<T>())
                    {
                        var handler = (IIntegrationEventHandler<T>)scope.ServiceProvider.GetRequiredService(handlerType);
                        await handler.HandleAsync(@event, _cts.Token);
                    }
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