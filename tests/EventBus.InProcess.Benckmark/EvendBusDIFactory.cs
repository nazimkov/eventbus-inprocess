using EventBus.InProcess.Benchmark.Events;
using EventBus.InProcess.Benchmark.Handlers;
using EventBus.InProcess.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.InProcess.Benchmark
{
    internal class EvendBusDIFactory
    {
        private Type[] _subscribers => new Type[]
        {
            typeof(UserInfoUpdatedHandler),
            typeof(UserInfoUpdatedHandlerOne),
            typeof(UserInfoUpdatedHandlerTwo),
            typeof(UserInfoUpdatedHandlerThree),
            typeof(UserInfoUpdatedHandlerFour),
        };

        public ServiceProvider ServiceProvider { get; private set; }

        public IEventBus GetBus(int subsQuantity)
        {
            if (subsQuantity > _subscribers.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(subsQuantity));
            }

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, subsQuantity);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            var bus = ServiceProvider.GetRequiredService<IEventBus>();
            AddSubsctibers(bus, subsQuantity);
            return bus;
        }

        private void ConfigureServices(IServiceCollection services, int subsQuantity)
        {
            services.AddSingleton<EventRecorder>();
            services.AddEventBus();
            RegisterHandlers(services, subsQuantity);
        }

        private void RegisterHandlers(IServiceCollection services, int subsQuantity)
        {
            for (int i = 0; i < subsQuantity; i++)
            {
                var subType = _subscribers[i];
                services.AddScoped(subType);
            }
        }

        private void AddSubsctibers(IEventBus bus, int subsQuantity)
        {
            for (int i = 0; i < subsQuantity; i++)
            {
                var subType = _subscribers[i];

                var method = typeof(IEventBus).GetMethod(nameof(IEventBus.Subscribe));
                var generic = method.MakeGenericMethod(typeof(UserInfoUpdatedEvent), subType);
                generic.Invoke(bus, null);
            }
        }
    }
}