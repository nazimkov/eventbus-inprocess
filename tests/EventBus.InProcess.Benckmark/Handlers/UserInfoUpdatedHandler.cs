using EventBus.InProcess.Benchmark.Events;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Benchmark.Handlers
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

    internal class UserInfoUpdatedHandlerOne : UserInfoUpdatedHandler
    {
        public UserInfoUpdatedHandlerOne(EventRecorder eventRecorder) : base(eventRecorder)
        {
        }
    }

    internal class UserInfoUpdatedHandlerTwo : UserInfoUpdatedHandler
    {
        public UserInfoUpdatedHandlerTwo(EventRecorder eventRecorder) : base(eventRecorder)
        {
        }
    }

    internal class UserInfoUpdatedHandlerThree : UserInfoUpdatedHandler
    {
        public UserInfoUpdatedHandlerThree(EventRecorder eventRecorder) : base(eventRecorder)
        {
        }
    }

    internal class UserInfoUpdatedHandlerFour : UserInfoUpdatedHandler
    {
        public UserInfoUpdatedHandlerFour(EventRecorder eventRecorder) : base(eventRecorder)
        {
        }
    }
}