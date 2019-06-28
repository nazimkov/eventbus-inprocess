using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    internal class EventBusInProcess : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subsManager;
        private readonly IChannelManager _channelManager;
        private readonly IServiceScopeFactory _scopeFactory;

        public EventBusInProcess(
            IEventBusConfig config,
            IEventBusSubscriptionManager subsManager,
            IChannelManager channelManager,
            IServiceScopeFactory scopeFactory)
        {
            _subsManager = subsManager ?? new InMemorySubscriptionManager();
            _channelManager = channelManager ?? new ChannelManager();
            _scopeFactory = scopeFactory;
        }

        public void Publish<T>(T @event) where T : IntegrationEvent
        {
            var channel = _channelManager.GetOrCreateChannel<T>();
            channel.Writer.WriteAsync(@event).GetAwaiter().GetResult();
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            _subsManager.AddSubscription<T, TH>();
            var channel = _channelManager.GetOrCreateChannel<T>();

            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public async Task ProcessEvent<T>(T @event) where T : IntegrationEvent
        {
            if (_subsManager.HasSubscriptionsForEvent<T>())
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    foreach(var hanlerType in _subsManager.GetHandlersForEvent<T>())
                    {
                        var handler = (IIntegrationEventHandler<T>)scope.ServiceProvider.GetRequiredService(hanlerType);
                        await handler.Handle(@event);
                    }
                }
            }
        }
    }
}