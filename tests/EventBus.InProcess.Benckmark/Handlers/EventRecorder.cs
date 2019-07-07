using System.Threading;

namespace EventBus.InProcess.Benchmark.Handlers
{
    public class EventRecorder
    {
        private int _numberHandledEvents;

        public int NumberHandledEvents
        {
            get { return _numberHandledEvents; }
        }

        public void ResetCounter() => _numberHandledEvents = 0;

        public void EventHandled() => Interlocked.Increment(ref _numberHandledEvents);
    }
}