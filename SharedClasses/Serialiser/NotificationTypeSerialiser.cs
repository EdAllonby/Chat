using System.Diagnostics.Contracts;
using System.Net.Sockets;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Serialise and deserialise a <see cref="NotificationType"/> when a notification <see cref="IMessage"/> is serialised.
    /// </summary>
    internal sealed class NotificationTypeSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NotificationType));

        private readonly BinarySerialiser serialiser = new BinarySerialiser();

        public void Serialise(NetworkStream networkStream, NotificationType notificationType)
        {
            Contract.Requires(networkStream != null);

            serialiser.Serialise(networkStream, notificationType);

            Log.DebugFormat("Sent Message NotificationType: {0} to stream", notificationType);
        }

        public NotificationType Deserialise(NetworkStream stream)
        {
            Contract.Requires(stream != null);
            var notificationType = (NotificationType) serialiser.Deserialise(stream);
            return notificationType;
        }
    }
}