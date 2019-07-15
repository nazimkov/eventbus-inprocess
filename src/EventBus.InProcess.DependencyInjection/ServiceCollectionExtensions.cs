using EventBus.InProcess.Internals;
using EventBus.InProcess.Internals.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.InProcess.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionManager, InMemorySubscriptionManager>();
            services.AddSingleton<IChanneslManager, ThreadChanelsManager>();
            services.AddSingleton<IEventProcessor, EventProcessorDI>();
            services.AddSingleton<IEventBus, EventBusInProcess>();

            return services;
        }
    }
}