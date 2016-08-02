using System.Net.Sockets;
using log4net;
using SharedClasses.Domain;

namespace SharedClasses.Serialiser.EntitySerialiser
{
    /// <summary>
    /// Used to serialise and deserialise the <see cref="Conversation" /> Domain object.
    /// </summary>
    internal sealed class ConversationSerialiser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConversationSerialiser));

        private readonly ISerialisationType serialiser = new BinarySerialiser();

        public void Serialise(NetworkStream networkStream, Conversation conversation)
        {
            serialiser.Serialise(networkStream, conversation);
            Log.Debug("Conversation serialised and sent to network stream");
        }

        public Conversation Deserialise(NetworkStream networkStream)
        {
            var conversation = (Conversation) serialiser.Deserialise(networkStream);
            Log.Debug("Network stream has received data and deserialised to a Conversation object");
            return conversation;
        }
    }
}