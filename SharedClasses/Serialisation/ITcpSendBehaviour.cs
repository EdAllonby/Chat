using System.IO;

namespace SharedClasses.Serialisation
{
    public interface ITcpSendBehaviour
    {
        /// <summary>
        /// Method used to serialise the object data over the TCP Socket
        /// </summary>
        void Serialise(Stream networkStream, Message clientMessage);

        /// <summary>
        /// Method to get incoming data and deserialise it into a Message object
        /// </summary>
        /// <returns>A Message object</returns>
        Message Deserialise(Stream networkStream);
    }
}