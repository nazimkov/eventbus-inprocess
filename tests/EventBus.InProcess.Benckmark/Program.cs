using EventBus.InProcess.Benckmark.Events;
using EventBus.InProcess.Benckmark.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.InProcess.Benckmark
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // create service provider
            serviceCollection.AddScoped<UserInfoUpdatedHandler>();
            serviceCollection.AddScoped<UserInfoUpdatedHandlerOne>();
            serviceCollection.AddScoped<UserInfoUpdatedHandlerTwo>();
            serviceCollection.AddScoped<UserInfoUpdatedHandlerThree>();
            serviceCollection.AddScoped<UserInfoUpdatedHandlerFour>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var bus = serviceProvider.GetRequiredService<IEventBus>();
            bus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandler>();

            var benchmark = serviceProvider.GetService<EventBusBenchmark>();

            RunBenchmark(benchmark, 1, "One subscriber");
            bus.Dispose();


            var multiSubsProvider = serviceCollection.BuildServiceProvider();
            var multiSubsBus = multiSubsProvider.GetRequiredService<IEventBus>();

            multiSubsBus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandler>();
            multiSubsBus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerOne>();
            multiSubsBus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerTwo>();
            multiSubsBus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerThree>();
            multiSubsBus.Subscribe<UserInfoUpdatedEvent, UserInfoUpdatedHandlerFour>();

            var multiSubsBenchmark = multiSubsProvider.GetService<EventBusBenchmark>();

            RunBenchmark(multiSubsBenchmark, 5, "Five subscribers");

            Console.ReadLine();
        }

        private static void RunBenchmark(EventBusBenchmark benchmark, int subsNumber, string name)
        {
            Console.WriteLine($"=== Running {name} benchmark ===");
            benchmark.Run(_ => { }, 100, subsNumber); // Warmup

            foreach (var eventsNumber in new[] { 1000, 10000, 100000, 1000000 })
            {
                benchmark.Run(Console.WriteLine, eventsNumber, subsNumber);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EventRecorder>();
            services.AddTransient<EventBusBenchmark>();
            services.AddEventBus();
        }
    }
}
