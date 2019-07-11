using EventBus.InProcess.Internals.Channels;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class ThreadChannelTests
    {
        private readonly TestChannel<string> _channel;
        private readonly ThreadChannel<string> _threadChannel;

        public ThreadChannelTests()
        {
            _channel = new TestChannel<string>();
            _threadChannel = new ThreadChannel<string>(_channel);
        }

        [Fact]
        public void Ctor_ChannelTaskNotNull_ChannelIsCreated()
        {
            // Arrange
            var channel = new TestChannel<string>();

            // Act
            var threadChannel = new ThreadChannel<string>(channel);

            // Assert
            Assert.NotNull(threadChannel);
        }

        [Fact]
        public void Ctor_ChannelTaskIsNull_ThrowsArgumentNullException()
        {
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new ThreadChannel<string>(null));
        }

        [Fact]
        public async Task WriteAsync_ValidMessage_MessageIsWriAsync()
        {
            // Arrange

            // Act
            await _threadChannel.WriteAsync("event", CancellationToken.None);

            // Assert
            Assert.True(_channel.IsWritten);
        }

        [Fact]
        public void WriteAsync_MessageIsNull_ThrowsArgumentNullException()
        {
            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => _threadChannel.WriteAsync(null, CancellationToken.None));
        }

        [Fact]
        public async Task ReadUntilCancelledAsync_Receiver_MessageIsRead()
        {
            // Arrange
            var pause = new ManualResetEvent(false);
            Func<string, ValueTask> testReceiver = _ => { pause.Set(); return new ValueTask(); };
            
            // Act
            await _threadChannel.ReadUntilCancelledAsync(testReceiver, CancellationToken.None);

            // Assert
            Assert.True(pause.WaitOne(100));
        }


        [Fact]
        public async Task ReadUntilCancelledAsync_ReadCanceled_ReadingIsStopped()
        {
            // Arrange
            const int MaxCount = 100000;
            const int CountToCancell = 100;

            var counter = 0;
            var cts = new CancellationTokenSource();
            var channel = new TestChannel<string>(MaxCount);
            var threadChannel = new ThreadChannel<string>(channel);

            Func<string, ValueTask> testReceiver = _ => 
            {
                if(counter >= CountToCancell)
                {
                    cts.Cancel();
                }
                counter++;
                return new ValueTask();
            };

            // Act
            await threadChannel.ReadUntilCancelledAsync(testReceiver, cts.Token);

            // Assert
            Assert.True(CountToCancell <= counter && counter <= MaxCount);
        }

        private class TestChannel<T> : Channel<T>
        {
            public bool IsWritten => _testChannelWriter.IsWritten;

            private TestChannelWriter _testChannelWriter;
            private TestChannelReader _testChannelReader;

            public TestChannel(int maxCount = 1)
            {
                _testChannelWriter = new TestChannelWriter();
                _testChannelReader = new TestChannelReader();
                _testChannelReader.MaxCount = maxCount;
                Writer = _testChannelWriter;
                Reader = _testChannelReader;
            }

            private class TestChannelWriter : ChannelWriter<T>
            {
                public bool IsWritten { get; private set; }

                public override bool TryWrite(T item)
                {
                    throw new NotImplementedException();
                }

                public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
                {
                    throw new NotImplementedException();
                }

                public override ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
                {
                    IsWritten = true;
                    return new ValueTask();
                }
            }

            private class TestChannelReader : ChannelReader<T>
            {
                private int _counter;

                public int MaxCount {get; set;}

                public override bool TryRead(out T item)
                {
                    _counter++;
                    item = default;
                    return _counter <= MaxCount;
                }

                public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
                {
                    return new ValueTask<bool>(_counter <= MaxCount);
                }
            }
        }
    }
}