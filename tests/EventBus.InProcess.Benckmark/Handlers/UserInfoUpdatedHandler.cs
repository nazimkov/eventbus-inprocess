using EventBus.InProcess.Benckmark.Events;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Benckmark.Handlers
{
    internal class UserInfoUpdatedHandler : IIntegrationEventHandler<UserInfoUpdatedEvent>
    {
        private readonly EventRecorder _eventRecorder;

        public UserInfoUpdatedHandler(EventRecorder eventRecorder)
        {
            _eventRecorder = eventRecorder;
        }

        public Task HandleAsync(UserInfoUpdatedEvent @event, CancellationToken cancellationToken)
        {
            _eventRecorder.EventHandled();
            return Task.CompletedTask;
        }
    }
}