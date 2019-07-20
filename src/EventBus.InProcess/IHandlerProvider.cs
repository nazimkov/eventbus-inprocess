using System;

namespace EventBus.InProcess
{
    public interface IHandlerProvider
    {
        THandler GetInstance<THandler>();
        object GetInstance(Type type);
    }
}