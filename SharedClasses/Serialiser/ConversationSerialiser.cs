using System.Diagnostics.Contracts;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="Conversation" /> Domain object
    /// Both <see cref="NewConversationRequest" /> and <see cref="ParticipationsNotification" /> use this class
    /// to do its the main serialisation work
    /// </summary>
    internal sealed class ConversationSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Serialise(Conversation conversation, NetworkStream networkStream)
        {
            Contract.Requires(conversation != null);
            Contract.Requires(networkStream != null);

            binaryFormatter.Serialize(networkStream, conversation);
            Log.Debug("Conversation serialised and sent to network stream");
        }

        public Conversation Deserialise(NetworkStream networkStream)
        {
            Contract.Requires(networkStream != null);

            var conversation = (Conversation) binaryFormatter.Deserialize(networkStream);
            Log.Debug("Network stream has received data and deserialised to a Conversation object");
            return conversation;
        }
    }
}