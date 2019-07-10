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
        public async Task ReadUntilCancelledAsync_ValidReceiver_MessageIsRead()
        {
            // Arrange
            Func<string, ValueTask> dummyReceiver = _ => new ValueTask();
            
            // Act
            await _threadChannel.ReadUntilCancelledAsync(dummyReceiver, CancellationToken.None);

            // Assert
            Assert.True(_channel.IsRead);
        }

        private class TestChannel<T> : Channel<T>
        {
            public bool IsWritten => _testChannelWriter.IsWritten;
            public bool IsRead => _testChannelReader.IsRead;

            private TestChannelWriter _testChannelWriter;
            private TestChannelReader _testChannelReader;

            public TestChannel()
            {
                _testChannelWriter = new TestChannelWriter();
                _testChannelReader = new TestChannelReader();
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
                public bool IsRead { get; private set; }
                private _oneTime = true;
                public override bool TryRead(out T item)
                {
                    IsRead = true;
                    item = default;
                    return true;
                }

                public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
                {
                    return new ValueTask<bool>(false);
                }
            }
        }
    }
}