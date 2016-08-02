using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Defines an <see cref="IMessage" /> MessageSerialiser together.
    /// Each <see cref="IMessageSerialiser" /> can only serialise and deserialise an <see cref="IMessage" />.
    /// This was created to allow the <see cref="SerialiserFactory" /> class to pick the correct MessageSerialiser.
    /// </summary>
    public interface IMessageSerialiser
    {
        /// <summary>
        /// Serialise an <see cref="IMessage" /> down the NetworkStream.
        /// </summary>
        /// <param name="networkStream">The networkStream that connects the Client and Server.</param>
        /// <param name="message">The <see cref="IMessage" /> to send.</param>
        void Serialise(NetworkStream networkStream, IMessage message);

        /// <summary>
        /// Deserialise an <see cref="IMessage" /> from the NetworkStream.
        /// </summary>
        /// <param name="networkStream">The networkStream that connects the Client and Server.</param>
        /// <returns>An <see cref="IMessage" />.</returns>
        IMessage Deserialise(NetworkStream networkStream);
    }
}