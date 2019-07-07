using BenchmarkDotNet.Running;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Linq;

namespace EventBus.InProcess.Benchmark
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var app = new CommandLineApplication();

            var typeValues = ((int[])Enum.GetValues(typeof(BenchmarkType)));
            var benchmarkTypesPromptValues = typeValues
                .Select(v => v.ToString())
                .Zip(Enum.GetNames(typeof(BenchmarkType)), (v, t) => $"{v} - {t}")
                .Aggregate((v, r) => $"{v}, {r}");

            app.OnExecute(() =>
            {
                var proceed = true;
                while (proceed)
                {
                    var benchTypeInt = Prompt.GetInt(
                        $"Select type[{benchmarkTypesPromptValues}]:",
                        (int)BenchmarkType.PoorMan,
                        ConsoleColor.White,
                        ConsoleColor.DarkBlue);

                    if (!Enum.IsDefined(typeof(BenchmarkType), benchTypeInt))
                    {
                        var allowedValues = string.Join(',', typeValues);
                        Console.WriteLine($"Entered benchmark type: {benchTypeInt} is invalid, allowed values {allowedValues}");
                        continue;
                    }

                    switch ((BenchmarkType)benchTypeInt)
                    {
                        case BenchmarkType.BenchmarkDotNet:
                            BenchmarkRunner.Run<EventBusInProcessBenchmark>();
                            break;

                        case BenchmarkType.PoorMan:
                            PoorManBenchmarkRunner.Run(1, 3, 5);
                            break;
                    }

                    proceed = Prompt.GetYesNo("Do you want to proceed?", false, ConsoleColor.Green);
                }
            });

            app.Execute(args);
        }

        private enum BenchmarkType
        {
            BenchmarkDotNet,
            PoorMan
        }
    }
}