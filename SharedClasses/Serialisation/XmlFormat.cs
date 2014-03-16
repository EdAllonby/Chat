using System;
using System.IO;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace SharedClasses.Serialisation
{
    public class XmlFormat : ITcpSendBehaviour
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(typeof (XmlFormat));

        public void Serialise(NetworkStream networkStream, Message clientMessage)
        {
            var serialiser = new XmlSerializer(typeof (Message));

            var streamWriter = new StreamWriter(networkStream, System.Text.Encoding.UTF8);

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