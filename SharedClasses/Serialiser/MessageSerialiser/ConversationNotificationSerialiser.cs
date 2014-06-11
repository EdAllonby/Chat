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

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ConversationNotification message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageIdentifier.ConversationNotification, networkStream);

            Log.DebugFormat("Waiting for {0} message to serialise", message.MessageIdentifier);
            conversationSerialiser.Serialise(networkStream, message.Conversation);
            Log.InfoFormat("{0} message serialised", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversation = new ConversationNotification(conversationSerialiser.Deserialise(networkStream));
            Log.InfoFormat("{0} message deserialised", conversation.MessageIdentifier);
            return conversation;
        }
    }
}