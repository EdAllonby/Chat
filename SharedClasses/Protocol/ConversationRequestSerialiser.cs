using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to serialise and deserialise a <see cref="ConversationRequest" /> message
    /// </summary>
    internal class ConversationRequestSerialiser : ISerialiser<ConversationRequest>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRequestSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();
        private readonly MessageIdentifierSerialiser messageIdentifierSerialiser = new MessageIdentifierSerialiser();

        #region Serialise

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

        #endregion

        #region Deserialise

        public IMessage Deserialise(NetworkStream networkStream)
        {
            var request = (ConversationRequest) binaryFormatter.Deserialize(networkStream);
            Log.Info("Conversation request message deserialised");
            return request;
        }

        #endregion
    }
}