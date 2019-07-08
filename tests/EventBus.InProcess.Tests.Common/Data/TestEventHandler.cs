using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Tests.Common.Data
{
    public class TestEventHandler : IIntegrationEventHandler<TestEvent>
    {
        private readonly Action<TestEvent> _callback;

        public TestEventHandler(Action<TestEvent> callback)
        {
            _callback = callback;
        }

        public Task HandleAsync(TestEvent @event, CancellationToken token)
        {
            _callback.Invoke(@event);
            return Task.CompletedTask;
        }
    }
}