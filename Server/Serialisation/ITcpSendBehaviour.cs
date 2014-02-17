namespace Server.Serialisation
{
    public interface ITcpSendBehaviour
    {
        /// <summary>
        /// Method used to serialise the object data over the TCP Socket
        /// </summary>
        void Serialise(Client clientMessage);

        /// <summary>
        /// Method to get incoming data and deserialise it into a Client object
        /// </summary>
        /// <returns>A Client object</returns>
        Client Deserialise();
    }
}