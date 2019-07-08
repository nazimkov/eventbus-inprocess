using System;

namespace EventBus.InProcess.Tests.Common.Data
{
    public class SecondTestEventHandler : TestEventHandler
    {
        public SecondTestEventHandler(Action<TestEvent> callback) : base(callback)
        {
        }
    }
}