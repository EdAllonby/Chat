using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="NewConversationRequest" /> message.
    /// </summary>
    internal sealed class NewConversationRequestSerialiser : Serialiser<NewConversationRequest>
    {
        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        protected override void Serialise(NewConversationRequest message, NetworkStream networkStream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageIdentifier.NewConversationRequest, networkStream);

            Log.DebugFormat("Waiting for {0} message to serialise", message.MessageIdentifier);
            binaryFormatter.Serialize(networkStream, message);
            Log.InfoFormat("{0} message serialised", message.MessageIdentifier);
        }

        public override IMessage Deserialise(NetworkStream networkStream)
        {
            var conversation = (NewConversationRequest) binaryFormatter.Deserialize(networkStream);
            Log.InfoFormat("{0} message deserialised", conversation.MessageIdentifier);
            return conversation;
        }
    }
}