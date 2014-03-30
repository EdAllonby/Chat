using System.Net.Sockets;

namespace SharedClasses.Protocol
{
    public interface ISerialiser
    {
        void Serialise(IMessage message, NetworkStream stream);

        IMessage Deserialise(NetworkStream stream);
    }
}
