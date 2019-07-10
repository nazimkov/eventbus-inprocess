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
            _channel = channel;
        }

        public async ValueTask WriteAsync(T @event, CancellationToken cancellationToken)
        {
            await _channel.Writer.WriteAsync(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}