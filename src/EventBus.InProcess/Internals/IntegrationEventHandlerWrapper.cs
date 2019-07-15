using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    internal abstract class IntegrationEventHandlerWrapper<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        public abstract ValueTask HandleAsync(
            TIntegrationEvent @event,
            IServiceFactory serviceFactory,
            CancellationToken cancellationToken = default);
    }
}