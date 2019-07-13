using System;
using System.Collections.Concurrent;

namespace EventBus.InProcess
{
    internal class DafaultServiceFactory : IServiceFactory
    {
        private readonly ConcurrentDictionary<Type, object> _handlers = new ConcurrentDictionary<Type, object>();

        public THandler GetInstance<THandler>() => (THandler)GetInstance(typeof(THandler));

        public object GetInstance(Type type)
        {
            return _handlers.GetOrAdd(type, t => Activator.CreateInstance(type));
        }
    }
}