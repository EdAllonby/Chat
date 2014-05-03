using System.Net.Sockets;
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

        private readonly ConversationSerialiser conversationSerialiser = new ConversationSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ConversationNotification message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ConversationNotification, stream);

            Log.Debug("Waiting for a contribution notification message to serialise");
            conversationSerialiser.Serialise(message.Conversation, stream);
            Log.Info("Contribution notification message serialised");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((ConversationNotification) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var notification = new ConversationNotification(conversationSerialiser.Deserialise(networkStream));
            Log.Info("Conversation notification message deserialised");
            return notification;
        }
    }
}