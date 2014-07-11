using System.Net.Sockets;
using SharedClasses.Message;
using SharedClasses.Serialiser.EntitySerialiser;

namespace SharedClasses.Serialiser.MessageSerialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ConversationRequest" /> message.
    /// </summary>
    internal sealed class ConversationRequestSerialiser : Serialiser<ConversationRequest>
    {
        private readonly ISerialisationType serialiser = new BinarySerialiser();

        private readonly ParticipationSerialiser participationSerialiser = new ParticipationSerialiser();

        protected override void Serialise(NetworkStream networkStream, ConversationRequest message)
        {
            Log.DebugFormat("Waiting for {0} message to serialise", message.MessageIdentifier);
            serialiser.Serialise(networkStream, message);
            Log.InfoFormat("{0} message serialised", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversation = (ConversationRequest) serialiser.Deserialise(networkStream);
            Log.InfoFormat("{0} message deserialised", conversation.MessageIdentifier);

            return conversation;
        }
    }
}