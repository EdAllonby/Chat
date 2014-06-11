using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ConversationSnapshotRequest" /> object.
    /// </summary>
    internal sealed class ConversationSnapshotRequestSerialiser : Serialiser<ConversationSnapshotRequest>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ConversationSnapshotRequest message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(message.MessageIdentifier, networkStream);

            binaryFormatter.Serialize(networkStream, message);
            Log.InfoFormat("{0} serialised and sent to network stream", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversationSnapshotRequest = (ConversationSnapshotRequest) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("Network stream has received data and deserialised to a {0} object", conversationSnapshotRequest.MessageIdentifier);
            return conversationSnapshotRequest;
        }
    }
}