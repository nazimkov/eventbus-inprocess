using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals.Channels
{
    public interface IChannel<T>
    {
        ValueTask WriteAsync(T @event, CancellationToken cancellationToken);
        ValueTask ReadUntilCancelledAsync(Func<T, ValueTask> receiver, CancellationToken cancellationToken);
    }


}