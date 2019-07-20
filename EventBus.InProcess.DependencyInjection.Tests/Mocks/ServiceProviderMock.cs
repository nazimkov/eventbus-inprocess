using Moq;
using System;
using System.Collections.Generic;

namespace EventBus.InProcess.DependencyInjection.Tests.Mocks
{
    internal static class ServiceProviderMock
    {
        public static Mock<IServiceProvider> GetMock(IDictionary<Type, object> services)
        {
            var serviceProvider = new Mock<IServiceProvider>();

            foreach (var service in services)
            {
                serviceProvider
               .Setup(x => x.GetService(service.Key))
               .Returns(service.Value);
            }

            return serviceProvider;
        }
    }
}