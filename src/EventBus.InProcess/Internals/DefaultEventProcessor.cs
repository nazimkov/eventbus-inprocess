using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    internal class DefaultEventProcessor : EventProcessor
    {
        private readonly IServiceFactory _serviceFactory;

        public DefaultEventProcessor()
        {
            _serviceFactory = new DafaultServiceFactory();
        }

        public override Task ProcessEventAsync<T>(T @event, IEnumerable<Type> hadlersTypes, CancellationToken cancellationToken)
        {
            return ProcessEventAsync(@event, hadlersTypes, _serviceFactory, cancellationToken);
        }
    }
}