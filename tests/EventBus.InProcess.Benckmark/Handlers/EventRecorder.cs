using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Benckmark.Handlers
{
    public class EventRecorder
    {
        private int _eventsHandled;

        public int EventsHandled
        {
            get { return _eventsHandled; }
        }


        public EventRecorder()
        {
        }

        public void EventHandled()
        {
            Interlocked.Increment(ref _eventsHandled);
        }
    }
}