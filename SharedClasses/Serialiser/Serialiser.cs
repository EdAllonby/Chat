using System.Net.Sockets;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Generic Serialiser which serialises a type which inherits <see cref="IMessage" /> down the stream
    /// </summary>
    /// <typeparam name="T"><see cref="IMessage" /> object which is any message that can be used in this protocol</typeparam>
    public abstract class Serialiser<T> : ISerialiser where T : IMessage
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(Serialiser<T>));

        /// <summary>
        /// Serialise <see cref="T"/> down the wire.
        /// </summary>
        /// <param name="message">The message which inherits from <see cref="IMessage" />.</param>
        /// <param name="stream">The stream that connects the Client and Server.</param>
        protected abstract void Serialise(T message, NetworkStream stream);

        /// <summary>
        /// Serialise the <see cref="IMessage"/> down the wire.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send.</param>
        /// <param name="stream">The stream that connects the Client and Server.</param>
        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((T) message, stream);
        }

        /// <summary>
        /// Deserialises an <see cref="IMessage"/> that has been received.
        /// </summary>
        /// <param name="stream">The stream that connects the Client and Server.</param>
        /// <returns>The <see cref="IMessage"/> that was received from the stream.</returns>
        public abstract IMessage Deserialise(NetworkStream stream);
    }
}