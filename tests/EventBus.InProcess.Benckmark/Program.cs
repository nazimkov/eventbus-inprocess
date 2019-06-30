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

            serviceProvider.GetService<EventBusBenchmark>().Run(Console.WriteLine);
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
