using System;
using System.Collections.Concurrent;
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
        private readonly ConcurrentDictionary<int, User> usersIndexedById = new ConcurrentDictionary<int, User>();

        public event EventHandler<User> UserAdded;
        public event EventHandler<User> UserConnectionUpdated;
        public event EventHandler<User> UserAvatarUpdated;


        public void AddUser(User user)
        {
            Contract.Requires(user != null);

            usersIndexedById.TryAdd(user.UserId, user);
            Log.DebugFormat("User with Id {0} added.", + user.UserId);

            OnUserAdded(user);
        }

        /// <summary>
        /// Updates a <see cref="User"/>'s <see cref="ConnectionStatus"/>
        /// </summary>
        /// <param name="userId"> The <see cref="User"/>'s Id to change connection status.</param>
        /// <param name="connectionStatus"> The new connection status of the user.</param>
        public void UpdateUserConnection(ConnectionStatus connectionStatus)
        {
            Contract.Requires(connectionStatus != null);

            User user = FindUserById(connectionStatus.UserId);
            user.ConnectionStatus = connectionStatus;

            OnUserConnectionUpdated(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avatar"></param>
        public void UpdateUserAvatar(Avatar avatar)
        {
            Contract.Requires(avatar != null);

            User user = FindUserById(avatar.UserId);

            user.Avatar = avatar;

            OnUserAvatarUpdated(user);
        }

        /// <summary>
        /// Adds <see cref="User"/> entities to the repository.
        /// </summary>
        /// <param name="users">The <see cref="User"/> entities to add to the repository.</param>
        public void AddUsers(IEnumerable<User> users)
        {
            Contract.Requires(users != null);

            IEnumerable<User> usersEnumerable = users as IList<User> ?? users.ToList();

            foreach (User user in usersEnumerable)
            {
                usersIndexedById[user.UserId] = user;
                Log.Debug("User with Id " + user.UserId + " added to user repository");
            }
        }

        /// <summary>
        /// Retrieves a <see cref="User"/> entity from the repository.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> entity ID to find.</param>
        /// <returns>The <see cref="User"/> which matches the ID. If no <see cref="User"/> is found, return null.</returns>
        public User FindUserById(int userId)
        {
            User user;

            return usersIndexedById.TryGetValue(userId, out user) ? user : null;
        }

        public User FindUserByUsername(string username)
        {
            return usersIndexedById.Where(user => user.Value.Username == username).Select(user => user.Value).FirstOrDefault();
        }

        /// <summary>
        /// Retrieves all <see cref="User"/> entities from the repository.
        /// </summary>
        /// <returns>A collection of all <see cref="User"/> entities in the repository.</returns>
        public IEnumerable<User> GetAllUsers()
        {
            return usersIndexedById.Values;
        }

        private void OnUserAdded(User user)
        {
            EventHandler<User> userAddedCopy = UserAdded;

            if (userAddedCopy != null)
            {
                userAddedCopy(this, user);
            }
        }

        private void OnUserConnectionUpdated(User user)
        {
            EventHandler<User> userConnectionUpdatedCopy = UserConnectionUpdated;

            if (userConnectionUpdatedCopy != null)
            {
                userConnectionUpdatedCopy(this, user);
            }
        }

        private void OnUserAvatarUpdated(User user)
        {
            EventHandler<User> userUpdatedCopy = UserAvatarUpdated;

            if (userUpdatedCopy != null)
            {
                userUpdatedCopy(this, user);
            }
        }
    }
}