using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="NewConversationRequest" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ConversationNotificationSerialiser : Serialiser<ConversationNotification>
    {
        private readonly ConversationSerialiser conversationSerialiser = new ConversationSerialiser();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ConversationNotification message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ConversationNotification, stream);

            Log.DebugFormat("Waiting for {0} message to serialise", message.Identifier);
            conversationSerialiser.Serialise(message.Conversation, stream);
            Log.InfoFormat("{0} message serialised", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var conversation = new ConversationNotification(conversationSerialiser.Deserialise(stream));
            Log.InfoFormat("{0} message deserialised", conversation.Identifier);
            return conversation;
        }
    }
}