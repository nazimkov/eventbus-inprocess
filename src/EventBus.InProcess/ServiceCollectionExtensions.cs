using EventBus.InProcess.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.InProcess
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionManager, InMemorySubscriptionManager>();
            services.AddSingleton<IChannelManager, ChannelManager>();
            services.AddSingleton<IEventBus, EventBusInProcess>();

            return services;
        }

        public static IServiceCollection AddSubscription<T, TH>(
            this IServiceCollection services, IServiceProvider provider)
            where TH : class, IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            var bus = provider.GetRequiredService<IEventBus>();

            bus.Subscribe<T, TH>();

            return services;
        }
    }
}