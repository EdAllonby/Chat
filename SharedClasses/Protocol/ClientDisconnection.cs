using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    class ClientDisconnection : IMessage
    {
        public ClientDisconnection()
        {
            Identifier = MessageNumber.ClientDisconnection;
        }

        public int Identifier { get; private set; }
    }
}
