using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;
using log4net;

namespace SharedClasses.Serialisation
{
    public class XmlFormat : ITcpSendBehaviour
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (XmlFormat));

        public void Serialise(NetworkStream networkStream, Message clientMessage)
        {
            var serialiser = new XmlSerializer(typeof (Message));

            var streamWriter = new StreamWriter(networkStream, Encoding.UTF8);

            serialiser.Serialize(streamWriter, clientMessage);
        }

        public Message Deserialise(NetworkStream networkStream)
        {
            try
            {
                var xmlSerialiser = new XmlSerializer(typeof (Message));
                Message client = null;

                if (networkStream.CanRead)
                {
                    client = (Message) xmlSerialiser.Deserialize(networkStream);
                }
                return client;
            }
            catch (Exception e)
            {
                log.Error("Cannot deserialise data - exception: ", e);
                return null;
            }
        }
    }
}