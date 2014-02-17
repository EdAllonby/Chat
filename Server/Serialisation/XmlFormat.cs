using System.IO;

namespace Server.Serialisation
{
    public class XmlFormat : ITcpSendBehaviour
    {
        public void Serialise(Client clientMessage)
        {
            throw new System.NotImplementedException();
        }

        public Client Deserialise(Stream networkStream)
        {
            throw new System.NotImplementedException();
        }
    }
}
