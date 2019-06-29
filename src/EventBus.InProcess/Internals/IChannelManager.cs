using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    public interface IChannelManager
    {
        Task<Channel<T>> CreateAsync<T>(Func<T, Task> receiver, CancellationToken cancellationToken);
        Channel<T> Get<T>();
    }
}