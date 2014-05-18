using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ConversationNotification" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ConversationNotificationSerialiser : ISerialiser<ConversationNotification>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationNotificationSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ConversationNotification message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ConversationNotification, stream);

            Log.Debug("Waiting for a contribution notification message to serialise");
            binaryFormatter.Serialize(stream, message);
            Log.Info("Contribution notification message serialised");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((ConversationNotification) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var conversationNotification = (ConversationNotification)binaryFormatter.Deserialize(networkStream);
            Log.Info("Conversation notification message deserialised");
            return conversationNotification;
        }
    }
}