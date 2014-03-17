using System.Net.Sockets;

namespace SharedClasses.Serialisation
{
    public interface ITcpSendBehaviour
    {
        /// <summary>
        ///     Method used to serialise the object data over the TCP Socket
        /// </summary>
        void Serialise(NetworkStream networkStream, Message clientMessage);

        /// <summary>
        ///     Method to get incoming data and deserialise it into a Message object
        /// </summary>
        /// <returns>A Message object</returns>
        Message Deserialise(NetworkStream networkStream);
    }
}