using EventBus.InProcess.Internals;
using EventBus.InProcess.Internals.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace EventBus.InProcess.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register <see cref="IEventBus"/> and related dependencies implementation
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            services.AddSingleton<IEventBusSubscriptionManager, InMemorySubscriptionManager>();
            services.AddSingleton<IChanneslManager, ThreadChanelsManager>();
            services.AddSingleton<IEventBus, EventBusInProcessDependencyInjection>();
            services.AddSingleton<IHandlerProvider, HandlerProviderDependencyInjection>();

            return services;
        }
    }
}