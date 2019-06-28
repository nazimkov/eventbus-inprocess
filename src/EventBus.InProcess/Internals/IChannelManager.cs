using System.Threading.Channels;

namespace EventBus.InProcess.Internals
{
    public interface IChannelManager
    {
        Channel<T> GetOrCreateChannel<T>();
    }
}