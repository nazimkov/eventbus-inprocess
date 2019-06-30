using EventBus.InProcess.Benckmark.Events;
using EventBus.InProcess.Benckmark.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.InProcess.Benckmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // create service provider
            serviceCollection.AddScoped<UserInfoUpdatedHandler>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceCollection.AddSubscription<UserInfoUpdatedEvent, UserInfoUpdatedHandler>(serviceProvider);

            var benchmark = serviceProvider.GetService<EventBusBenchmark>();

            benchmark.Run(_ => { }, 100); // Warmup

            foreach (var eventsNumber in new[] { 1000, 10000, 100000 })
            {
                benchmark.Run(Console.WriteLine, eventsNumber);
            }

            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EventRecorder>();
            services.AddTransient<EventBusBenchmark>();
            services.AddEventBus();
        }
    }
}
