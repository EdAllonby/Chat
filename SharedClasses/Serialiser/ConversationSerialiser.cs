using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="Conversation" /> Domain object
    /// Both <see cref="ConversationRequest" /> and <see cref="ConversationNotification" /> use this class
    /// to do its the main serialisation work
    /// </summary>
    internal sealed class ConversationSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationSerialiser));

        private readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        public void Serialise(Conversation conversation, NetworkStream stream)
        {
            if (stream.CanWrite)
            {
                Log.Info("Attempt to serialise Conversation and send to stream");
                binaryFormatter.Serialize(stream, conversation);
                Log.Info("Conversation serialised and sent to network stream");
            }
        }

        public Conversation Deserialise(NetworkStream networkStream)
        {
            var conversation = (Conversation) binaryFormatter.Deserialize(networkStream);
            Log.Info("Network stream has received data and deserialised to a Conversation object");
            return conversation;
        }
    }
}