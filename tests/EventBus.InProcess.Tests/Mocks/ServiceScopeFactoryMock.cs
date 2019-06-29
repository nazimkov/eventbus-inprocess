using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;

namespace EventBus.InProcess.Tests.Mocks
{
    internal static class ServiceScopeFactoryMock
    {
        public static Mock<IServiceScopeFactory> GetMock(IDictionary<Type, object> services)
        {
            var serviceProvider = new Mock<IServiceProvider>();

            foreach (var service in services)
            {
                serviceProvider
               .Setup(x => x.GetService(service.Key))
               .Returns(service.Value);
            }

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
                .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            return serviceScopeFactory;
        }
    }
}