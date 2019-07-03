using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    public static class ChannelReaderExtensions
    {
        public static async Task ReadUntilCancelledAsync<T>(this ChannelReader<T> reader, Func<T, Task> receiver,
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
            while (!cancellationToken.IsCancellationRequested
                && await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}