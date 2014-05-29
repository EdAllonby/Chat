using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ParticipationsNotification" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ParticipationsNotificationSerialiser : Serialiser<ParticipationsNotification>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ParticipationsNotification message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ParticipationsNotification, stream);

            Log.DebugFormat("Waiting for a {0} message to serialise", message.Identifier);
            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} message serialised", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var conversationNotification = (ParticipationsNotification) binaryFormatter.Deserialize(stream);
            Log.InfoFormat("{0} message deserialised", conversationNotification.Identifier);
            return conversationNotification;
        }
    }
}