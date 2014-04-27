using System.Collections.Generic;
using System.Linq;
using SharedClasses.Domain;

namespace SharedClasses
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Find a User in a List of Connected Clients by its ID.
        /// </summary>
        /// <param name="connectedClientCollection">The collection of ConnectedClients that will be searched</param>
        /// <param name="userID">The User ID to find in the ConnectedClient collection</param>
        /// <returns>The Connected Client that matches the User ID</returns>
        public static ConnectedClient FindConnectedClientByUserId(this IEnumerable<ConnectedClient> connectedClientCollection, int userID)
        {
            return connectedClientCollection.FirstOrDefault(connectedClient => connectedClient.User.UserId.Equals(userID));
        }
    }
}