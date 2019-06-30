using EventBus.InProcess.Benckmark.Events;
using EventBus.InProcess.Benckmark.Handlers;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Benckmark
{
    internal class EventBusBenchmark
    {
        private readonly IEventBus _bus;
        private readonly EventRecorder _eventRecorder;

        public EventBusBenchmark(IEventBus bus, EventRecorder eventRecorder)
        {
            _bus = bus;
            _eventRecorder = eventRecorder;
        }

        internal void Run(Action<string> writer)
        {
            const int runsCount = 100; 
            var tasks = new Task[runsCount];

            var watch = new Stopwatch();
            watch.Start();
            for (var i = 0; i < runsCount; i++)
            {
                tasks[i] = Task.Run(() => _bus.Publish(new UserInfoUpdatedEvent
                {
                    UpdatedUser = new User()
                }));
            }

            Task.WaitAll(tasks);

            while(watch.ElapsedMilliseconds < 10000)
            {

            }

            var elapsed = watch.Elapsed;
            watch.Stop();


            writer($"{_eventRecorder.EventsHandled} events handled in {elapsed}");
        }
    }
}