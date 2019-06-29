using EventBus.InProcess.Internals;
using System.Threading.Channels;
using Xunit;

namespace EventBus.InProcess.Tests
{
    public class ChannelManagerTests
    {
        private readonly ChannelManager _channelManager;

        public ChannelManagerTests()
        {
            _channelManager = new ChannelManager();
        }

        [Fact]
        public void GetOrCreate_NewMessageType_CreatesAndReturnsNewChannelOfMessageType()
        {
            // Arrange
            // Act
            var channel = _channelManager.GetOrCreate<ChannelMessage>();

            // Assert
            Assert.NotNull(channel);
            Assert.True(channel is Channel<ChannelMessage>);
        }

        [Fact]
        public void GetOrCreate_ExistingMessageType_ReturnsExistingChannel()
        {
            // Arrange
            var newChannel = _channelManager.GetOrCreate<ChannelMessage>();

            // Act
            var existingChannel = _channelManager.GetOrCreate<ChannelMessage>();

            // Assert
            Assert.True(ReferenceEquals(newChannel, existingChannel));
        }

        internal class ChannelMessage
        {
        }
    }
}