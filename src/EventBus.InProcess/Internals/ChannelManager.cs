using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace EventBus.InProcess.Internals
{
    public class ChannelManager : IChannelManager
    {
        private readonly Dictionary<Type, ValueTuple<object, CancellationTokenSource>> _channels;
        private readonly CancellationTokenSource _internalCts;

        public ChannelManager()
        {
            _channels = new Dictionary<Type, ValueTuple<object, CancellationTokenSource>>();
            _internalCts = new CancellationTokenSource();
        }

        public async Task<Channel<T>> CreateAsync<T>(Func<T, Task> receiver, CancellationToken cancellationToken)
        {
            var eventType = typeof(T);

            if (HasChannel<T>())
            {
                return Get<T>();
            }

            var newChannel = Channel.CreateUnbounded<T>();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _internalCts.Token);

            // TODO Consider to move from Task per Channel to one Channel for all events
            await Task.Factory.StartNew(async () =>
                await newChannel.Reader.ReadUntilCancelledAsync(receiver, linkedCts.Token),
                linkedCts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default).ConfigureAwait(false);

            _channels.Add(eventType, (newChannel, linkedCts));

            return newChannel;
        }

        public Channel<T> Get<T>()
        {
            var (channel, _) = GetChannelWithCts<T>();

            return (Channel<T>)channel;
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
                throw new ArgumentException($"Channel for type {typeof(T).Name} does't exists", nameof(T));
            }

            return channelWithCts;
        }

        private bool HasChannel<T>() => _channels.ContainsKey(typeof(T));

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
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