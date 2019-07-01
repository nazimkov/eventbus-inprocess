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
        private readonly Func<ChannelMessage, Task> _dummyReceiver = _ => Task.CompletedTask;

        public ChannelManagerTests()
        {
            _channelManager = new ChannelManager();
        }

        [Fact]
        public async Task Create_NewMessageType_CreatesAndReturnsNewChannelOfMessageTypeAsync()
        {
            // Arrange
            // Act
            var channel = await _channelManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Assert
            Assert.NotNull(channel);
            Assert.True(channel is Channel<ChannelMessage>);
        }

        [Fact]
        public async Task Create__ExistingMessageType_ReturnsExistingChannel()
        {
            var newChannel = await _channelManager.CreateAsync(_dummyReceiver, CancellationToken.None);

            // Act
            var existingChannel = _channelManager.Get<ChannelMessage>();

            // Assert
            Assert.True(ReferenceEquals(newChannel, existingChannel));
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
        {
        }
    }
}