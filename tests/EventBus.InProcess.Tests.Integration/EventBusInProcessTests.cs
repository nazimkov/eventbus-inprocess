using EventBus.InProcess.Tests.Integration.Common;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests.Integration
{
    public class EventBusInProcessTests : EventBusInProcessIntegrationTests
    {
        private readonly EventBusInProcess _bus;
        private readonly DelegateHandlerProvider _handlerProvider;

        public EventBusInProcessTests()
        {
            _handlerProvider = new DelegateHandlerProvider();
            _bus = new EventBusInProcess(_handlerProvider);
        }

        [Fact]
        public async Task PublishAsync_EventWithHandler_HandlerReceivesEvent()
        {
            await Test_PublishAsync_EventWithHandler_HandlerReceivesEvent(() => _bus);
        }

        [Fact]
        public async Task PublishAsync_EventWithMultipleHandlers_AllHandlersReceiveEvent()
        {
            await Test_PublishAsync_EventWithMultipleHandlers_AllHandlersReceiveEvent(() => _bus);
        }

        protected override void AddHandlerBuilder(Type handlerType, HandlerBuilder handlerBuilder = null)
        {
            _handlerProvider.AddHandlerBuilder(handlerType, handlerBuilder);
        }
    }
}