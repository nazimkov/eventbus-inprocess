using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals.Channels
{
    internal sealed class ThreadChannel<T> : IChannel<T>
    {
        private readonly Channel<T> _channel;

        public ThreadChannel(Channel<T> channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public async ValueTask ReadUntilCancelledAsync(Func<T, ValueTask> receiver, CancellationToken cancellationToken)
        {
            await ReadUntilCancelledAsync(_channel.Reader, receiver, cancellationToken);
        }

        public ValueTask WriteAsync(T @event, CancellationToken cancellationToken)
        {
            if(@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            return _channel.Writer.WriteAsync(@event, cancellationToken);
        }

        private async ValueTask ReadUntilCancelledAsync(ChannelReader<T> reader, Func<T, ValueTask> receiver,
            CancellationToken cancellationToken)
        {
            do
            {
                while (!cancellationToken.IsCancellationRequested &&
                    reader.TryRead(out var item))
                {
                    await receiver(item);
                }
            }
            while (!cancellationToken.IsCancellationRequested &&
                await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}