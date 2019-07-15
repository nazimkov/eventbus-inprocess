using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess
{
    public interface IEventProcessor
    {
        Task ProcessEventAsync<T>(T @event, IEnumerable<Type> hadlersTypes, CancellationToken cancellationToken)
            where T : IntegrationEvent;
    }
}