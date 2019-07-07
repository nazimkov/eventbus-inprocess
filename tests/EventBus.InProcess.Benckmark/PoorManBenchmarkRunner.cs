using EventBus.InProcess.Benckmark.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.InProcess.Benckmark
{
    internal static class PoorManBenchmarkRunner
    {
        public static void Run(params int[] subsQty)
        {
            foreach (var qty in subsQty)
            {
                SetupAndRun(qty);
            }
        }

        private static void SetupAndRun(int subsQty)
        {
            var busFactory = new EvendBusDIFactory();
            var bus = busFactory.GetBus(subsQuantity: 1);
            var eventRecorder = busFactory.ServiceProvider.GetRequiredService<EventRecorder>();
            var benchmark = new EventBusPoorManBenchmark(bus, eventRecorder);

            RunBenchmark(benchmark, 1);
            bus.Dispose();
        }

        private static void RunBenchmark(EventBusPoorManBenchmark benchmark, int subsQty)
        {
            Console.WriteLine($"=== Running benchmark for {subsQty} subscribers ===");
            benchmark.Run(_ => { }, 100, subsQty); // Warmup

            foreach (var eventsNumber in new[] { 1000, 10000, 100000, 1000000 })
            {
                benchmark.Run(Console.WriteLine, eventsNumber, subsQty);
            }
        }
    }
}