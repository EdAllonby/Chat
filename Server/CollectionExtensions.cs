using System.Collections.Generic;
using System.Linq;

namespace Server
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Find a User in a List of Connected Clients by its ID.
        /// </summary>
        /// <param name="clientHandlerCollection">The collection of <see cref="ClientHandler"/> that will be searched</param>
        /// <param name="userId">The User ID to find in the <see cref="ClientHandler"/> collection</param>
        /// <returns>The <see cref="ClientHandler"/> that matches the User's ID</returns>
        public static ClientHandler FindClientHandlerByUserId(this IEnumerable<ClientHandler> clientHandlerCollection, int userId)
        {
            return clientHandlerCollection.FirstOrDefault(clientHandler => clientHandler.ClientUser.UserId.Equals(userId));
        }
    }
}