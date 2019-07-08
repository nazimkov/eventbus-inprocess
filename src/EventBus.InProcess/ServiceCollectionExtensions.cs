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
            services.AddSingleton<IChanneslManager, ThreadingChanelsManager>();
            services.AddSingleton<IEventBus, EventBusInProcess>();

            return services;
        }
    }
}