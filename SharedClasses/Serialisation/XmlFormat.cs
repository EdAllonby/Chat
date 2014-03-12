using System;
using System.IO;
using System.Xml.Serialization;

namespace SharedClasses.Serialisation
{
    public class XmlFormat : ITcpSendBehaviour
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Serialise(Stream networkStream, Message clientMessage)
        {
            var serialiser = new XmlSerializer(typeof(Message));
            var memoryStream = new MemoryStream();

            var streamWriter = new StreamWriter(networkStream, System.Text.Encoding.UTF8);

            serialiser.Serialize(streamWriter, clientMessage);
        }

        public Message Deserialise(Stream networkStream)
        {
            try
            {
                while (true)
                {
                    var xmlSerialiser = new XmlSerializer(typeof(Message));
                    Message client = null;

                    if (networkStream.CanRead)
                    {
                        client = (Message)xmlSerialiser.Deserialize(networkStream);
                    }
                    return client;
                }
            }
            catch (Exception e)
            {
                log.Error("Cannot deserialise data - exception: ", e);
                return null;
            }
        }
    }
}
