using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ConversationNotification" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ConversationNotificationSerialiser : Serialiser<ConversationNotification>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ConversationNotification message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ConversationNotification, stream);

            Log.DebugFormat("Waiting for a {0} message to serialise", message.Identifier);
            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} message serialised", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var conversationNotification = (ConversationNotification) binaryFormatter.Deserialize(stream);
            Log.InfoFormat("{0} message deserialised", conversationNotification.Identifier);
            return conversationNotification;
        }
    }
}