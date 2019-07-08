using Microsoft.Extensions.DependencyInjection;

namespace EventBus.InProcess.Tests.Integration
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

        public void RegisterHandler<THandler>(THandler instance)
            where THandler : class
        {
            ServiceCollection.AddScoped(b => instance);
        }
    }
}