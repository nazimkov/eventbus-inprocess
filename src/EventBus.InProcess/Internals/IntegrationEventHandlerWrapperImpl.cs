using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    internal class IntegrationEventHandlerWrapperImpl<TIntegrationEvent, THandler> : IntegrationEventHandlerWrapper<TIntegrationEvent>
        where TIntegrationEvent : IntegrationEvent
        where THandler : IIntegrationEventHandler<TIntegrationEvent>
    {
        public override Task HandleAsync(
            TIntegrationEvent @event,
            IServiceFactory serviceFactory,
            CancellationToken cancellationToken = default)
        {
            var hanler = GetHandler(serviceFactory);
            return hanler.HandleAsync(@event, cancellationToken);
        }

        private THandler GetHandler(IServiceFactory serviceFactory)
        {
            THandler handler;

            try
            {
                handler = serviceFactory.GetInstance<THandler>();
            }
            catch (Exception ex)
            {
                var message = GetFullErrorMessage("Error constructing handler for request of type");
                throw new InvalidOperationException(message, ex);
            }

            if (handler == null)
            {
                var message = GetFullErrorMessage("Handler was not found for request of type");
                throw new InvalidOperationException(message);
            }

            return handler;

            string GetFullErrorMessage(string message)
            {
                const string RegisterHandlersErrorMessage = "You should register your handlers first.";
                return string.Format("{0} {1}. {2}", message, typeof(THandler), RegisterHandlersErrorMessage);
            }
        }
    }
}