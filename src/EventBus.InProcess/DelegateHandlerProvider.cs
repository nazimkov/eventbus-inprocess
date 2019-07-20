using System;
using System.Collections.Concurrent;

namespace EventBus.InProcess
{
    public sealed class DelegateHandlerProvider : IHandlerProvider
    {
        private readonly ConcurrentDictionary<Type, object> _handlers = new ConcurrentDictionary<Type, object>();
        private readonly ConcurrentDictionary<Type, HandlerBuilder> _handlerBuilders = new ConcurrentDictionary<Type, HandlerBuilder>();

        public void AddHandlerBuilder(Type type, HandlerBuilder builder)
        {
            _handlerBuilders.TryAdd(type, builder);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public THandler GetInstance<THandler>() => (THandler)GetInstance(typeof(THandler));

        public object GetInstance(Type type)
        {
            if (_handlers.TryGetValue(type, out var handler))
            {
                return handler;
            }

            if (_handlerBuilders.TryGetValue(type, out var resolver))
            {
                return _handlers.GetOrAdd(type, resolver.Invoke());
            }

            return _handlers.GetOrAdd(type, _ => Activator.CreateInstance(type));
        }
    }
}