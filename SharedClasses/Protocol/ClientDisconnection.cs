namespace SharedClasses.Protocol
{
    /// <summary>
    /// Used to describe a client disconnection
    /// Note that this has no <see cref="ISerialiser"/> associated with it.
    /// This is because a Client has no way of sending this to a server,
    /// therefore it is generated when the Server decides a Client has disconnected
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