namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to describe a client disconnection
    /// </summary>
    internal sealed class ClientDisconnection : IMessage
    {
        public ClientDisconnection()
        {
            Identifier = MessageNumber.ClientDisconnection;
        }

        public int Identifier { get; private set; }
    }
}