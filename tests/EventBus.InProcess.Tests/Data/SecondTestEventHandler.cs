using System;

namespace EventBus.InProcess.Tests.Data
{
    internal class SecondTestEventHandler : TestEventHandler
    {
        public SecondTestEventHandler(Action<TestEvent> callback) : base(callback)
        {
        }
    }
}