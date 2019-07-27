
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.InProcess.DependencyInjection.Tests.Integration
{
    public class ServiceCollectionFixture
    {
        public ServiceCollection ServiceCollection { get; }

        public ServiceCollectionFixture()
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddEventBus();
        }

        public IEventBus GetEventBus()
        {
            var serviceProvider = ServiceCollection.BuildServiceProvider();
            return serviceProvider.GetRequiredService<IEventBus>();
        }

        public void RegisterHandler(Type handlerType, HandlerBuilder handlerBuilder)
        {
            ServiceCollection.AddTransient(handlerType, _ => handlerBuilder());
        }
    }
}