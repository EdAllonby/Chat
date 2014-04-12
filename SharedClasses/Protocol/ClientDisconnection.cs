namespace SharedClasses.Protocol
{
    internal class ClientDisconnection : IMessage
    {
        public ClientDisconnection()
        {
            Identifier = MessageNumber.ClientDisconnection;
        }

        public int Identifier { get; private set; }
    }
}