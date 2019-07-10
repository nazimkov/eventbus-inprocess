using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals.Channels
{
    public interface IChanneslManager : IDisposable
    {
        Task<IChannel<T>> CreateAsync<T>(Func<T, ValueTask> receiver, CancellationToken cancellationToken);

        IChannel<T> Get<T>();

        void DisposeChannel<T>();
    }
}