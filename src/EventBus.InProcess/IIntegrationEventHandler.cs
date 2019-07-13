using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess
{
    public interface IIntegrationEventHandler<in TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
    {
        Task HandleAsync(TIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}