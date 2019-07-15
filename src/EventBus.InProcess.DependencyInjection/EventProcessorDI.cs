using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.DependencyInjection
{
    public class EventProcessorDI : EventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EventProcessorDI(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public override async Task ProcessEventAsync<T>(T @event, IEnumerable<Type> hadlersTypes, CancellationToken cancellationToken)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var serviceFactory = new ServiceFactoryDI(scope.ServiceProvider);

                await ProcessEventAsync(@event, hadlersTypes, serviceFactory, cancellationToken);
            }
        }
    }
}