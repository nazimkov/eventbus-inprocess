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
            this IServiceCollection services)
            where TH : class, IIntegrationEventHandler<T>
            where T : IntegrationEvent
        {
            services.AddScoped<TH>();

            var bus = services
                .BuildServiceProvider() // TODO Rethink building service provider each time
                .GetRequiredService<IEventBus>();

            bus.Subscribe<T, TH>();

            return services;
        }
    }
}