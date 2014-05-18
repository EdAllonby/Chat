using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ConversationRequest" /> message
    /// Uses <see cref="ConversationSerialiser" /> for its underlying serialiser
    /// </summary>
    internal sealed class ConversationRequestSerialiser : ISerialiser<ConversationRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRequestSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        public void Serialise(ConversationRequest message, NetworkStream stream)
        {
            messageIdentifierSerialiser.SerialiseMessageIdentifier(MessageNumber.ConversationRequest, stream);

            Log.Debug("Waiting for conversation request message to serialise");
            binaryFormatter.Serialize(stream, message);
            Log.Info("Conversation request message serialised");
        }

        public void Serialise(IMessage message, NetworkStream stream)
        {
            Serialise((ConversationRequest) message, stream);
        }

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var conversation = (ConversationRequest) binaryFormatter.Deserialize(networkStream);
            Log.Info("Conversation request message deserialised");
            return conversation;
        }
    }
}