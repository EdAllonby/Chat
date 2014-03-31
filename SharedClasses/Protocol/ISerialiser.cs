using System.Net.Sockets;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Groups the message serialisers to an <see cref="ISerialiser"/> type.
    /// This was created to allow the <see cref="SerialiserFactory"/> class to pick the correct serialiser
    /// </summary>
    public interface ISerialiser
    {
        /// <summary>
        /// Serialise an <see cref="IMessage"/> down the NetworkStream
        /// </summary>
        /// <param name="notificationMessage">The message to send</param>
        /// <param name="stream">The stream that connects the Client and Server</param>
        void Serialise(IMessage notificationMessage, NetworkStream stream);

        /// <summary>
        /// Deserialise an <see cref="IMessage"/> from the NetworkStream
        /// </summary>
        /// <param name="stream">The stream that connects the Client and Server</param>
        /// <returns>an <see cref="IMessage"/> object</returns>
        IMessage Deserialise(NetworkStream stream);
    }
}