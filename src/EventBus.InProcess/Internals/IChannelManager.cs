using System.Threading.Channels;

namespace EventBus.InProcess.Internals
{
    public interface IChannelManager
    {
        Channel<T> GetOrCreate<T>();
    }
}