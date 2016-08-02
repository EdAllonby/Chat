using System.Net.Sockets;
using SharedClasses.Message;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to Serialise and Deserialise a <see cref="ConversationSnapshotRequest" /> object.
    /// </summary>
    internal sealed class ConversationSnapshotRequestSerialiser : Serialiser<ConversationSnapshotRequest>
    {
        private readonly ISerialisationType serialiser = new BinarySerialiser();

        protected override void Serialise(NetworkStream networkStream, ConversationSnapshotRequest message)
        {
            serialiser.Serialise(networkStream, message);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversationSnapshotRequest = (ConversationSnapshotRequest) serialiser.Deserialise(networkStream);
            Log.InfoFormat($"Network stream has received data and deserialised to a {conversationSnapshotRequest.MessageIdentifier} object");
            return conversationSnapshotRequest;
        }
    }
}