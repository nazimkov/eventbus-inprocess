using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess
{

    public abstract class EventProcessor : IEventProcessor
    {
        public abstract Task ProcessEventAsync<T>(T @event, IEnumerable<Type> hadlersTypes, CancellationToken cancellationToken)
            where T : IntegrationEvent;

        protected async Task ProcessEventAsync<T>(T @event, IEnumerable<Type> hadlersTypes, IServiceFactory serviceFactory, CancellationToken cancellationToken)
            where T : IntegrationEvent
        {
            foreach (var handlerType in hadlersTypes)
            {
                var handler = (IntegrationEventHandlerWrapper<T>)Activator.CreateInstance(typeof(IntegrationEventHandlerWrapperImpl<,>).MakeGenericType(typeof(T), handlerType));
                await handler.HandleAsync(@event, serviceFactory, cancellationToken);
            }
        }

    }
}