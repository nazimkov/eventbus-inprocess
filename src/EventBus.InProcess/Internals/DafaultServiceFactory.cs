using System;
using System.Collections.Concurrent;

namespace EventBus.InProcess
{
    internal class DafaultServiceFactory : IServiceFactory
    {
        private readonly ConcurrentDictionary<Type, object> _handlers = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, HandlerBuilder> _handlersResolvers = new ConcurrentDictionary<Type, HandlerBuilder>();

        public void AddHandlerBuilder(Type type, HandlerBuilder builder)
        {
            _handlersResolvers.TryAdd(type, builder);
        }

        public THandler GetInstance<THandler>() => (THandler)GetInstance(typeof(THandler));

        public object GetInstance(Type type)
        {
            if (_handlers.TryGetValue(type, out var handler))
            {
                return handler;
            }

            if (_handlersResolvers.TryGetValue(type, out var resolver))
            {
                return _handlers.GetOrAdd(type, resolver.Invoke());
            }
            return _handlers.GetOrAdd(type, t => Activator.CreateInstance(type));
        }
    }
}