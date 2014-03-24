using System.Net.Sockets;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    public interface ITcpSendBehaviour
    {
        /// <summary>
        ///     Method used to serialise the object data over the TCP Socket
        /// </summary>
        void Serialise(NetworkStream networkStream, Contribution clientContribution);

        /// <summary>
        ///     Method to get incoming data and deserialise it into a Contribution object
        /// </summary>
        /// <returns>A Contribution object</returns>
        Contribution Deserialise(NetworkStream networkStream);
    }
}