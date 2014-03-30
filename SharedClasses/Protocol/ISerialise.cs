using System.Net.Sockets;

namespace SharedClasses.Protocol
{
    public interface ISerialise
    {
        void Serialise(IMessage message, NetworkStream stream);

        IMessage Deserialise(NetworkStream stream);
    }
}
