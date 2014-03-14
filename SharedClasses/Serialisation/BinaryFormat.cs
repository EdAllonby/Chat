using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharedClasses.Serialisation
{
    public class BinaryFormat : ITcpSendBehaviour
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Serialise(Stream networkStream, Message clientMessage)
        {
            var binaryFormatter = new BinaryFormatter();

            if (networkStream.CanWrite)
            {
                binaryFormatter.Serialize(networkStream, clientMessage);
            }
            networkStream.Close();
        }

        public Message Deserialise(Stream networkStream)
        {
            while (true)
            {
                var binaryFormatter = new BinaryFormatter();
                Message message = null;

                if (networkStream.CanRead)
                {
                    Log.Debug("Network stream can be read from, starting binary deserialisation process");

                    message = (Message) binaryFormatter.Deserialize(networkStream);

                    Log.Info("Message deserialised");
                }

                return message;
            }
        }
    }
}