using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace EventBus.InProcess.Internals
{
    public class ChannelManager : IChannelManager
    {
        private Dictionary<Type, object> _channels;
        public ChannelManager()
        {
            _channels = new Dictionary<Type, object>();
        }

        public Channel<T> GetOrCreate<T>()
        {
            var eventType = typeof(T);
 
            if(_channels.TryGetValue(eventType, out object channel))
            {
                return (Channel<T>)channel;
            }

            var newChannel = Channel.CreateUnbounded<T>();
            _channels.Add(eventType, newChannel);

            return newChannel;
        }
    }
}