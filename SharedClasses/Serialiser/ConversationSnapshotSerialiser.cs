using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ConversationSnapshot" /> object.
    /// </summary>
    internal sealed class ConversationSnapshotSerialiser : Serialiser<ConversationSnapshot>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ConversationSnapshot message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.Identifier, stream);

            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var conversationSnapshot = (ConversationSnapshot) binaryFormatter.Deserialize(stream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", conversationSnapshot.Identifier);
            return conversationSnapshot;
        }
    }
}