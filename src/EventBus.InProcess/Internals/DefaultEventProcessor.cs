using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    internal class DefaultEventProcessor : IEventProcessor
    {
        public async Task ProcessEventAsync<T>(T @event, IEnumerable<Type> hadlersTypes, IServiceFactory serviceFactory, CancellationToken cancellationToken)
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