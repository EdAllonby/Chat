using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharedClasses.Serialisation
{
    public class BinaryFormat : ITcpSendBehaviour
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(typeof (BinaryFormat));

        public void Serialise(NetworkStream networkStream, Message clientMessage)
        {
            var binaryFormatter = new BinaryFormatter();

            if (networkStream.CanWrite)
            {
                Log.Info("Attempt to serialise message and send to server");
                binaryFormatter.Serialize(networkStream, clientMessage);
                Log.Info("Message serialised and sent to network stream");
            }
        }

        public Message Deserialise(NetworkStream networkStream)
        {
            var binaryFormatter = new BinaryFormatter();
            Message message = null;

            if (networkStream.CanRead)
            {
                Log.Debug("Network stream can be read from, waiting for message");
                message = (Message) binaryFormatter.Deserialize(networkStream);
                Log.Info("Network stream has received data and deserialised to a message object");
            }

            return message;
        }
    }
}