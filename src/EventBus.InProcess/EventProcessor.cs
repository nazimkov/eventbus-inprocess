using EventBus.InProcess.Internals;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess
{
    public class EventProcessor : IEventProcessor
    {
        public async Task ProcessEventAsync<T>(T @event, IEnumerable<Type> hadlersTypes, IHandlerProvider handlerProvider, CancellationToken cancellationToken)
            where T : IntegrationEvent
        {
            foreach (var handlerType in hadlersTypes)
            {
                var handler = (IntegrationEventHandlerWrapper<T>)Activator.CreateInstance(typeof(IntegrationEventHandlerWrapperImpl<,>).MakeGenericType(typeof(T), handlerType));
                await handler.HandleAsync(@event, handlerProvider, cancellationToken);
            }
        }
    }
}