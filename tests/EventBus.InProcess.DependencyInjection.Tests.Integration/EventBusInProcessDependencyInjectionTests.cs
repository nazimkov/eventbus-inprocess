using EventBus.InProcess.Tests.Integration.Common;
using System;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.DependencyInjection.Tests.Integration
{
    public class EventBusInProcessDependencyInjectionTests : EventBusInProcessIntegrationTests, IClassFixture<ServiceCollectionFixture>
    {
        private readonly ServiceCollectionFixture _fixture;

        public EventBusInProcessDependencyInjectionTests(ServiceCollectionFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task PublishAsync_EventWithHandler_HandlerReceivesEvent()
        {
            await Test_PublishAsync_EventWithHandler_HandlerReceivesEvent(GetBusBuilder());
        }

        [Fact]
        public async Task PublishAsync_EventWithMultipleHandlers_AllHandlersReceiveEvent()
        {
            await Test_PublishAsync_EventWithMultipleHandlers_AllHandlersReceiveEvent(GetBusBuilder());
        }

        protected override void AddHandlerBuilder(Type handlerType, HandlerBuilder handlerBuilder = null)
        {
            _fixture.RegisterHandler(handlerType, handlerBuilder);
        }

        private Func<IEventBus> GetBusBuilder() => () => _fixture.GetEventBus();
    }
}