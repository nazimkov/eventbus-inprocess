using System;
using System.Collections.Concurrent;

namespace EventBus.InProcess
{
    public interface IServiceFactory
    {
        THandler GetInstance<THandler>();

        object GetInstance(Type type);
    }

    internal class DafaultServiceFactory : IServiceFactory
    {
        private readonly ConcurrentDictionary<Type, object> _handlers = new ConcurrentDictionary<Type, object>();

        public THandler GetInstance<THandler>() => (THandler)GetInstance(typeof(THandler));

        public object GetInstance(Type type)
        {
            var handler = _handlers.GetOrAdd(type, t => Activator.CreateInstance(type));
            return handler;
        }
    }
}