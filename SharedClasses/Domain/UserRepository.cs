using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using log4net;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Holds a collection of <see cref="User"/>s with basic CRUD operations.
    /// </summary>
    public sealed class UserRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserRepository));

        private readonly Dictionary<int, User> usersIndexedById = new Dictionary<int, User>();

        /// <summary>
        /// Adds or updates a <see cref="User"/> entity to the repository.
        /// </summary>
        /// <param name="user"><see cref="User"/> entity to add.</param>
        public void AddEntity(User user)
        {
            Contract.Requires(user != null);
            usersIndexedById[user.UserId] = user;

            if (usersIndexedById.ContainsKey(user.UserId))
            {
                Log.Debug("User with Id " + user.UserId + " has been updated");
            }
            else
            {
                Log.Debug("User with Id " + user.UserId + " added to user repository");
            }
        }

        /// <summary>
        /// Adds <see cref="User"/> entities to the repository.
        /// </summary>
        /// <param name="users">The <see cref="User"/> entities to add to the repository.</param>
        public void AddUsers(IEnumerable<User> users)
        {
            Contract.Requires(users != null);

            foreach (User user in users)
            {
                usersIndexedById[user.UserId] = user;
                Log.Debug("User with Id " + user.UserId + " added to user repository");
            }
        }

        /// <summary>
        /// Removes a <see cref="User"/> entity from the repository.
        /// </summary>
        /// <param name="userId"><see cref="User"/> entity to remove from the repository.</param>
        public void RemoveEntity(int userId)
        {
            usersIndexedById.Remove(userId);
            Log.Debug("User with Id " + userId + " removed from user repository");
        }

        /// <summary>
        /// Retrieves a <see cref="User"/> entity from the repository.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> entity ID to find.</param>
        /// <returns>The <see cref="User"/> which matches the ID. If no <see cref="User"/> is found, return null.</returns>
        public User FindEntityByID(int userId)
        {
            return usersIndexedById.ContainsKey(userId) ? usersIndexedById[userId] : null;
        }

        public User FindEntityByUsername(string username)
        {
            return usersIndexedById.Where(user => user.Value.Username == username).Select(user => user.Value).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all <see cref="User"/> entities from the repository.
        /// </summary>
        /// <returns>A collection of all <see cref="User"/> entities in the repository.</returns>
        public IEnumerable<User> GetAllEntities()
        {
            return usersIndexedById.Values.ToList();
        }
    }
}