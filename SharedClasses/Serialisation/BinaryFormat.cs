using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharedClasses.Serialisation
{
    public class BinaryFormat : ITcpSendBehaviour
    {
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
                    message = (Message)binaryFormatter.Deserialize(networkStream);
                }
                return message;
            }
        }
    }
}