using Server.Serialisation;

namespace Server
{
    public class XmlFormat : ITcpSendBehaviour
    {
        public void Serialise(Client clientMessage)
        {
            throw new System.NotImplementedException();
        }

        public Client Deserialise()
        {
            throw new System.NotImplementedException();
        }
    }
}
