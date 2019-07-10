using EventBus.InProcess.Internals.Channels;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class ThreadChanelsManagerTests
    {
        private readonly ThreadChanelsManager _channelsManager;
        private readonly Func<ChannelMessage, ValueTask> _dummyReceiver = _ => new ValueTask();

        public ThreadChanelsManagerTests()
        {
            _channelsManager = new ThreadChanelsManager();
        }

        [Fact]
        public async Task CreateAsync_NewMessageType_CreatesAndReturnsNewChannelOfMessageType()
        {
            // Arrange
            // Act
            var channel = await _channelsManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Assert
            Assert.NotNull(channel);
            Assert.True(channel is ThreadChannel<ChannelMessage>);
        }

        [Fact]
        public async Task CreateAsync_ExistingMessageType_ReturnsExistingChannel()
        {
            // Arrange
            var newChannel = await _channelsManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Act
            var existingChannel = _channelsManager.Get<ChannelMessage>();

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
            var newChannel = await _channelsManager.CreateAsync(callback, CancellationToken.None);

            // Act
            await newChannel.WriteAsync(new ChannelMessage(), CancellationToken.None);

            //Assert
            Assert.True(pause.WaitOne(100));
        }

        [Fact]
        public async Task Get_ExistingMessageType_ReturnsExistingChannel()
        {
            // Arrange
            var newChannel = await _channelsManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Act
            var existingChannel = _channelsManager.Get<ChannelMessage>();

            // Assert
            Assert.True(ReferenceEquals(newChannel, existingChannel));
        }

        [Fact]
        public void Get_NonExistingType_ThrowsArgumentException()
        {
            // Act
            // Assert
            Assert.Throws<ArgumentException>(() => _channelsManager.Get<TheoryAttribute>());
        }

        internal class ChannelMessage
        { }
    }
}