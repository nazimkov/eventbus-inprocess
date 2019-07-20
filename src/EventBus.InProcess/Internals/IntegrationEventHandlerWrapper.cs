using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    internal abstract class IntegrationEventHandlerWrapper<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        public abstract Task HandleAsync(
            TIntegrationEvent @event,
            IHandlerProvider serviceFactory,
            CancellationToken cancellationToken = default);
    }
}