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

        protected override void Serialise(ParticipationsNotification message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageIdentifier.ParticipationsNotification, networkStream);

            Log.DebugFormat("Waiting for a {0} message to serialise", message.MessageIdentifier);
            binaryFormatter.Serialize(networkStream, message);
            Log.InfoFormat("{0} message serialised", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversationNotification = (ParticipationsNotification) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("{0} message deserialised", conversationNotification.MessageIdentifier);
            return conversationNotification;
        }
    }
}