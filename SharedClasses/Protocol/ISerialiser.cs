using System.Net.Sockets;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Groups the message serialisers to an <see cref="ISerialiser" /> type.
    /// This was created to allow the <see cref="SerialiserFactory" /> class to pick the correct serialiser
    /// </summary>
    public interface ISerialiser
    {
        /// <summary>
        /// Serialise an <see cref="IMessage" /> down the NetworkStream
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="stream">The stream that connects the Client and Server</param>
        void Serialise(IMessage message, NetworkStream stream);

        /// <summary>
        /// Deserialise an <see cref="IMessage" /> from the NetworkStream
        /// </summary>
        /// <param name="networkStream">The stream that connects the Client and Server</param>
        /// <returns>an <see cref="IMessage" /> object</returns>
        IMessage Deserialise(NetworkStream networkStream);
    }

    /// <summary>
    /// Generic ISerialiser which serialises a type which inherits <see cref="IMessage" /> down the stream
    /// </summary>
    /// <typeparam name="T"><see cref="IMessage" /> object which is any message that can be used in this protocol</typeparam>
    public interface ISerialiser<T> : ISerialiser where T : IMessage
    {
        /// <summary>
        /// Serialise the <see cref="IMessage" /> down the wire
        /// </summary>
        /// <param name="message">The message which inherits from <see cref="IMessage" /></param>
        /// <param name="stream">The stream that connects the Client and Server</param>
        void Serialise(T message, NetworkStream stream);
    }
}