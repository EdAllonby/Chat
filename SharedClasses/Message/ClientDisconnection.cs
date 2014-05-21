using SharedClasses.Serialiser;

namespace SharedClasses.Message
{
    /// <summary>
    /// Used to describe a client disconnection
    /// Note that this has no <see cref="ISerialiser"/> associated with it.
    /// This is because a Client could have no way of sending this to a server,
    /// therefore it is generated when the Server decides a Client has disconnected
    /// by using the associated userID in the <see cref="ConnectionHandler"/>
    /// </summary>
    internal sealed class ClientDisconnection : IMessage
    {
        public MessageNumber Identifier
        {
            get { return MessageNumber.ClientDisconnection; }
        }
    }
}