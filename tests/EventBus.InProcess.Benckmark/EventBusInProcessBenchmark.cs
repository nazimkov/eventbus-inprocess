using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using EventBus.InProcess.Benchmark.Events;
using EventBus.InProcess.Benchmark.Handlers;
using System.Threading.Tasks;

namespace EventBus.InProcess.Benchmark
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 3, warmupCount: 5, targetCount: 50)]
    [RPlotExporter, RankColumn]
    public sealed class EventBusInProcessBenchmark
    {
        private EventRecorder _eventRecorder;
        private Task[] _pulishTasks;
        private IEventBus _bus;

        [Params(100, 10000, 1_000_000)]
        public int MessageQty;

        [Params(1, 5)]
        public int SubsQty;

        [GlobalSetup]
        public void Setup()
        {
            _eventRecorder = new EventRecorder();
            _pulishTasks = new Task[MessageQty];
            var busFactory = new EvendBusDIFactory();
            _bus = busFactory.GetBus(SubsQty);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _bus.Dispose();
        }

        [Benchmark]
        public int EventBusInProcess()
        {
            for (var i = 0; i < MessageQty; i++)
            {
                _pulishTasks[i] = Task.Run(() =>
                _bus.Publish(new UserInfoUpdatedEvent
                {
                    UpdatedUser = new User()
                }));
            }

            Task.WaitAll(_pulishTasks);

            var handledEventsNumber = MessageQty * SubsQty;
            while (_eventRecorder.NumberHandledEvents < handledEventsNumber)
            {
            }
            return _eventRecorder.NumberHandledEvents;
        }
    }
}