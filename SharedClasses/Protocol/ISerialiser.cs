using System.Net.Sockets;
using SharedClasses.Domain;

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
        /// <param name="message">The message to send</param>
        /// <param name="stream">The stream that connects the Client and Server</param>
        void Serialise(IMessage message, NetworkStream stream);

        /// <summary>
        /// Deserialise an <see cref="IMessage"/> from the NetworkStream
        /// </summary>
        /// <param name="stream">The stream that connects the Client and Server</param>
        /// <returns>an <see cref="IMessage"/> object</returns>
        IMessage Deserialise(NetworkStream stream);
    }

    public interface ISerialiser<T> : ISerialiser where T : IMessage
    {
        void Serialise(T message, NetworkStream stream);
    }
}