﻿using System;

namespace EventBus.InProcess
{
    public interface IServiceFactory
    {
        THandler GetInstance<THandler>();

        object GetInstance(Type type);
    }
}