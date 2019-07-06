using EventBus.InProcess.Internals;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class ChannelManagerTests
    {
        private readonly ChannelManager _channelManager;
        private readonly Func<ChannelMessage, ValueTask> _dummyReceiver = _ => new ValueTask();

        public ChannelManagerTests()
        {
            _channelManager = new ChannelManager();
        }

        [Fact]
        public async Task CreateAsync_NewMessageType_CreatesAndReturnsNewChannelOfMessageType()
        {
            // Arrange
            // Act
            var channel = await _channelManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Assert
            Assert.NotNull(channel);
            Assert.True(channel is Channel<ChannelMessage>);
        }

        [Fact]
        public async Task CreateAsync_ExistingMessageType_ReturnsExistingChannel()
        {
            // Arrange
            var newChannel = await _channelManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Act
            var existingChannel = _channelManager.Get<ChannelMessage>();

            // Assert
            Assert.True(ReferenceEquals(newChannel, existingChannel));
        }

        [Fact]
        public async Task CreateAsync_NewMessageType_TaskWithReadUntilStarted()
        {
            // Arrange
            var pause = new ManualResetEvent(false);
            Func<ChannelMessage, ValueTask> callback = _ =>
            {
                pause.Set();
                return new ValueTask();
            };
            var newChannel = await _channelManager.CreateAsync(callback, CancellationToken.None);

            // Act
            await newChannel.Writer.WriteAsync(new ChannelMessage());

            //Assert
            Assert.True(pause.WaitOne(100));
        }

        [Fact]
        public async Task Get_ExistingMessageType_ReturnsExistingChannel()
        {
            // Arrange
            var newChannel = await _channelManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Act
            var existingChannel = _channelManager.Get<ChannelMessage>();

            // Assert
            Assert.True(ReferenceEquals(newChannel, existingChannel));
        }

        [Fact]
        public void Get_NonExistingType_ThrowsArgumentException()
        {
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => _channelManager.Get<TheoryAttribute>());
        }

        internal class ChannelMessage
        { }
    }
}