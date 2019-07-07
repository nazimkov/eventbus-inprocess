using System;

namespace EventBus.InProcess
{
    public class EventRemovedArgs : EventArgs
    {
        public EventRemovedArgs(Type @event)
        {
            Event = @event;
        }

        public Type Event { get; }
    }
}