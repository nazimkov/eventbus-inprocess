using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals.Channels
{
    public sealed class ThreadChanelsManager : IChanneslManager
    {
        private readonly Dictionary<Type, ValueTuple<object, CancellationTokenSource>> _channels;
        private readonly CancellationTokenSource _internalCts;

        public ThreadChanelsManager()
        {
            _channels = new Dictionary<Type, ValueTuple<object, CancellationTokenSource>>();
            _internalCts = new CancellationTokenSource();
        }

        public async Task<IChannel<T>> CreateAsync<T>(Func<T, ValueTask> receiver, CancellationToken cancellationToken)
        {
            var eventType = typeof(T);

            if (HasChannel<T>())
            {
                return Get<T>();
            }

            var newChannel = Channel.CreateUnbounded<T>();
            var threadChannel = new ThreadChannel<T>(newChannel);

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _internalCts.Token);

            await Task.Factory.StartNew(async () =>
                await threadChannel.ReadUntilCancelledAsync(receiver, linkedCts.Token),
                linkedCts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).ConfigureAwait(false);

            _channels.Add(eventType, (threadChannel, linkedCts));

            return threadChannel;
        }

        public IChannel<T> Get<T>()
        {
            var (channel, _) = GetChannelWithCts<T>();

            return (ThreadChannel<T>)channel;
        }

        public void DisposeChannel<T>()
        {
            var (_, cts) = GetChannelWithCts<T>();

            using (cts) // Dispose linked CTS
            {
                cts.Cancel();
            }

            _channels.Remove(typeof(T));
        }

        private (object, CancellationTokenSource) GetChannelWithCts<T>()
        {
            var eventType = typeof(T);

            if (!_channels.TryGetValue(eventType, out var channelWithCts))
            {
                throw new ArgumentException($"Channel for type {typeof(T).Name} doesn't exists", nameof(T));
            }

            return channelWithCts;
        }

        private bool HasChannel<T>() => _channels.ContainsKey(typeof(T));

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _internalCts.Cancel();
                    _channels.Clear();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}