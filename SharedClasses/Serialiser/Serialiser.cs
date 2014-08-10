using System.Net.Sockets;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Generic Serialiser which serialises a type which inherits <see cref="IMessage" /> down the networkStream
    /// </summary>
    /// <typeparam name="T"><see cref="IMessage" /> object which is any message that can be used in this protocol</typeparam>
    public abstract class Serialiser<T> : ISerialiser where T : IMessage
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof (Serialiser<T>));

        /// <summary>
        /// Serialise the <see cref="IMessage" /> down the wire.
        /// </summary>
        /// <param name="networkStream">The stream that connects the Client and Server.</param>
        /// <param name="message">The <see cref="IMessage" /> to send.</param>
        public void Serialise(NetworkStream networkStream, IMessage message)
        {
            MessageIdentifierSerialiser.Serialise(networkStream, message.MessageIdentifier);
            Log.InfoFormat("Serialising an {0} across the network stream.", message.MessageIdentifier);
            Serialise(networkStream, (T) message);
        }

        /// <summary>
        /// Deserialises an <see cref="IMessage" /> that has been received.
        /// </summary>
        /// <param name="networkStream">The stream that connects the Client and Server.</param>
        /// <returns>The <see cref="IMessage" /> that was received from the networkStream.</returns>
        public abstract IMessage Deserialise(NetworkStream networkStream);

        /// <summary>
        /// Serialise <see cref="T" /> down the wire.
        /// </summary>
        /// <param name="networkStream">The networkStream that connects the Client and Server.</param>
        /// <param name="message">The message which inherits from <see cref="IMessage" />.</param>
        protected abstract void Serialise(NetworkStream networkStream, T message);
    }
}