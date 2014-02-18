using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server.Serialisation
{
    public class BinaryFormat : ITcpSendBehaviour
    {
        public void Serialise(Stream networkStream, Client clientMessage)
        {
            var binaryFormatter = new BinaryFormatter();

            if (networkStream.CanWrite)
            {
                binaryFormatter.Serialize(networkStream, clientMessage);
            }
            networkStream.Close();
        }

        public Client Deserialise(Stream networkStream)
        {
            while (true)
            {
                var binaryFormatter = new BinaryFormatter();
                Client client = null;

                if (networkStream.CanRead)
                {
                    client = (Client) binaryFormatter.Deserialize(networkStream);
                }
                return client;
            }
        }
    }
}