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
        public static ConnectedClient FindByUserID(this IEnumerable<ConnectedClient> connectedClientCollection, int userID)
        {
            return connectedClientCollection.FirstOrDefault(connectedClient => connectedClient.User.ID.Equals(userID));
        }

        /// <summary>
        /// Find a User in a List of Users by its ID.
        /// </summary>
        /// <param name="connectedUserCollection">The collection of Users that will be searched</param>
        /// <param name="userID">The User ID to find in the User collection</param>
        /// <returns>The User that matches the User ID</returns>
        public static User FindByUserID(this IEnumerable<User> connectedUserCollection, int userID)
        {
            return connectedUserCollection.FirstOrDefault(s => s.ID.Equals(userID));
        }
    }
}