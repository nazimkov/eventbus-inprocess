using EventBus.InProcess.Benckmark.Events;
using EventBus.InProcess.Benckmark.Handlers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EventBus.InProcess.Benckmark
{
    internal sealed class EventBusPoorManBenchmark
    {
        private readonly IEventBus _bus;
        private readonly EventRecorder _eventRecorder;

        public EventBusPoorManBenchmark(IEventBus bus, EventRecorder eventRecorder)
        {
            _bus = bus;
            _eventRecorder = eventRecorder;
        }

        internal void Run(Action<string> writer, int eventsNumber, int subsNumber)
        {
            _eventRecorder.ResetCounter();
            var tasks = new Task[eventsNumber];

            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < eventsNumber; i++)
            {
                tasks[i] = Task.Run(() => _bus.Publish(new UserInfoUpdatedEvent
                {
                    UpdatedUser = new User()
                }));
            }

            Task.WaitAll(tasks);

            var handledEventsNumber = eventsNumber * subsNumber;
            while (_eventRecorder.NumberHandledEvents < handledEventsNumber)
            {
            }

            var elapsed = watch.Elapsed;
            watch.Stop();

            writer($"{_eventRecorder.NumberHandledEvents} events handled in {elapsed.TotalMilliseconds}");
        }
    }
}