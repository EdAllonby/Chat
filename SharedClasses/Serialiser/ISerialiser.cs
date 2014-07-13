using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Groups the message serialisers together.
    /// Each <see cref="ISerialiser" /> can only serialise and deserialise an <see cref="IMessage"/> 
    /// This was created to allow the <see cref="SerialiserFactory" /> class to pick the correct serialiser
    /// </summary>
    public interface ISerialiser
    {
        MessageIdentifierSerialiser MessageIdentifierSerialiser { get; }

        /// <summary>
        /// Serialise an <see cref="IMessage" /> down the NetworkStream
        /// </summary>
        /// <param name="networkStream">The networkStream that connects the Client and Server</param>
        /// <param name="message">The message to send</param>
        void Serialise(NetworkStream networkStream, IMessage message);

        /// <summary>
        /// Deserialise an <see cref="IMessage" /> from the NetworkStream
        /// </summary>
        /// <param name="networkStream">The networkStream that connects the Client and Server</param>
        /// <returns>an <see cref="IMessage" /> object</returns>
        IMessage Deserialise(NetworkStream networkStream);
    }
}