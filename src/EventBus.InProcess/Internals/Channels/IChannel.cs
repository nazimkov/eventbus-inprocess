using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals.Channels
{
    public interface IChannel<T>
    {
        ValueTask WriteAsync(T @event, CancellationToken cancellationToken);
    }


}