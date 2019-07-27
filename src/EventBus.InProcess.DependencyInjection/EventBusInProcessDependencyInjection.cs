using EventBus.InProcess.Internals.Channels;

namespace EventBus.InProcess.DependencyInjection
{
    public class EventBusInProcessDependencyInjection : EventBusInProcess
    {
        public EventBusInProcessDependencyInjection(
            IEventBusSubscriptionManager subsManager,
            IChanneslManager channelManager,
            IHandlerProvider handlerProvider) : base(subsManager, channelManager, handlerProvider)
        {
        }
    }
}