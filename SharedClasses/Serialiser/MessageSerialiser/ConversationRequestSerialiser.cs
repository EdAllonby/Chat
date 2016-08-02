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
        private readonly ParticipationSerialiser participationSerialiser = new ParticipationSerialiser();
        private readonly ISerialisationType serialiser = new BinarySerialiser();

        protected override void Serialise(NetworkStream networkStream, ConversationRequest message)
        {
            serialiser.Serialise(networkStream, message);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversation = (ConversationRequest) serialiser.Deserialise(networkStream);
            Log.InfoFormat($"{conversation.MessageIdentifier} message deserialised");

            return conversation;
        }
    }
}