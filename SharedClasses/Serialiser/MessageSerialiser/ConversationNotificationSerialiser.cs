using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="NewConversationRequest" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ConversationNotificationSerialiser : Serialiser<ConversationNotification>
    {
        private readonly ConversationSerialiser conversationSerialiser = new ConversationSerialiser();
        private readonly NotificationTypeSerialiser notificationTypeSerialiser = new NotificationTypeSerialiser();

        protected override void Serialise(NetworkStream networkStream, ConversationNotification message)
        {
            Log.DebugFormat("Waiting for {0} message to serialise", message.MessageIdentifier);
            notificationTypeSerialiser.Serialise(networkStream, message.NotificationType);
            conversationSerialiser.Serialise(networkStream, message.Conversation);
            Log.InfoFormat("{0} message serialised", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            NotificationType notificationType = notificationTypeSerialiser.Deserialise(networkStream);
            var conversation = new ConversationNotification(conversationSerialiser.Deserialise(networkStream), notificationType);

            Log.InfoFormat("{0} message deserialised", conversation.MessageIdentifier);

            return conversation;
        }
    }
}