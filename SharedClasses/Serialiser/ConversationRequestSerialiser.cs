using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ConversationRequest" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ConversationRequestSerialiser : Serialiser<ConversationRequest>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(ConversationRequest message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ConversationRequest, stream);

            Log.DebugFormat("Waiting for {0} message to serialise", message.Identifier);
            binaryFormatter.Serialize(stream, message);
            Log.InfoFormat("{0} message serialised", message.Identifier);
        }

        public override IMessage Deserialise(NetworkStream stream)
        {
            var conversation = (ConversationRequest) binaryFormatter.Deserialize(stream);
            Log.InfoFormat("{0} message deserialised", conversation.Identifier);
            return conversation;
        }
    }
}