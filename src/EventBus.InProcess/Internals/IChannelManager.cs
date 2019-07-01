using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    public interface IChannelManager : IDisposable
    {
        Task<Channel<T>> CreateAsync<T>(Func<T, Task> receiver, CancellationToken cancellationToken);
        Channel<T> Get<T>();
        void DisposeChannel<T>();
    }
}