using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    public class ChannelManager : IChannelManager
    {
        private Dictionary<Type, object> _channels;

        public ChannelManager()
        {
            _channels = new Dictionary<Type, object>();
        }

        public async Task<Channel<T>> CreateAsync<T>(Func<T, Task> receiver, CancellationToken cancellationToken)
        {
            var eventType = typeof(T);

            var newChannel = Channel.CreateUnbounded<T>();

            await Task.Factory.StartNew(async () =>
                await ReadUntilCancelledAsync(newChannel.Reader, receiver, cancellationToken),
                TaskCreationOptions.LongRunning).ConfigureAwait(false);

            _channels.Add(eventType, newChannel);

            return newChannel;
        }

        public Channel<T> Get<T>()
        {
            var eventType = typeof(T);

            if (!_channels.TryGetValue(eventType, out object channel))
            {
                throw new InvalidOperationException("You should create channel first");
            }

            return (Channel<T>)channel;
        }

        private async Task ReadUntilCancelledAsync<T>(ChannelReader<T> reader, Func<T, Task> receiver,
           CancellationToken cancellationToken)
        {
            do
            {
                while (!cancellationToken.IsCancellationRequested
                    && reader.TryRead(out var item))
                {
                    await receiver(item);
                }
            }
            while (
                !cancellationToken.IsCancellationRequested
                && await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}