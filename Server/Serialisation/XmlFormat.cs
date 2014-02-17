using System.IO;
using Server.Serialisation;

namespace Server
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
