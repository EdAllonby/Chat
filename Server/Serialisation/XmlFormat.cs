using System;
using System.IO;
using System.Xml.Serialization;

namespace Server.Serialisation
{
    public class XmlFormat : ITcpSendBehaviour
    {
        public void Serialise(Stream networkStream, Client clientMessage)
        {
            var serialiser = new XmlSerializer(typeof(Client));
            var memoryStream = new MemoryStream();

            var streamWriter = new StreamWriter(networkStream, System.Text.Encoding.UTF8);

            serialiser.Serialize(streamWriter, clientMessage);
        }

        public Client Deserialise(Stream networkStream)
        {
            try
            {
                while (true)
                {
                    var xmlSerialiser = new XmlSerializer(typeof(Client));
                    Client client = null;

                    if (networkStream.CanRead)
                    {
                        client = (Client)xmlSerialiser.Deserialize(networkStream);
                    }
                    return client;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
