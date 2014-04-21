using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using SharedClasses.Domain;

namespace SharedClasses
{
    public sealed class ConnectedClient
    {
        public ConnectedClient(TcpClient tcpClient, User user)
        {
            TcpClient = tcpClient;
            User = user;
        }

        public TcpClient TcpClient { get; private set; }
        public User User { get; set; }

        /// <summary>
        /// Find a User in a List of Connected Clients by its ID.
        /// </summary>
        /// <param name="connectedClientCollection">The collection of ConnectedClients that will be searched</param>
        /// <param name="userID">The User ID to find in the ConnectedClient collection</param>
        /// <returns>The Connected Client that matches the User ID</returns>
        public static ConnectedClient FindByUserID(IEnumerable<ConnectedClient> connectedClientCollection, int userID)
        {
            return connectedClientCollection.FirstOrDefault(connectedClient => connectedClient.User.ID.Equals(userID));
        }
    }
}