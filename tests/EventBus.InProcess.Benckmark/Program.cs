using BenchmarkDotNet.Running;

namespace EventBus.InProcess.Benckmark
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<EventBusInProcessBenchmark>();
            PoorManBenchmarkRunner.Run(1, 3, 5);
        }
    }
}