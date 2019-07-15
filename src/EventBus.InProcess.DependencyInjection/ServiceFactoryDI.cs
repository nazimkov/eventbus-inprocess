using Microsoft.Extensions.DependencyInjection;
using System;

namespace EventBus.InProcess.DependencyInjection
{
    internal class ServiceFactoryDI : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactoryDI(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public THandler GetInstance<THandler>()
        {
            return _serviceProvider.GetRequiredService<THandler>();
        }

        public object GetInstance(Type type)
        {
            return _serviceProvider.GetRequiredService(type);
        }
    }
}