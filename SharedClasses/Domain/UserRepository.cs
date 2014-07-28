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

        public event EventHandler<EntityChangedEventArgs<User>> UserChanged;
        
        public void AddUser(User user)
        {
            Contract.Requires(user != null);

            usersIndexedById.TryAdd(user.Id, user);
            Log.DebugFormat("User with Id {0} added.", + user.Id);

            EntityChangedEventArgs<User> userChangedEventArgs = new EntityChangedEventArgs<User>();

            userChangedEventArgs.EntityAdded(user);

            OnUserChanged(userChangedEventArgs);
        }

        /// <summary>
        /// Updates a <see cref="User"/>'s <see cref="ConnectionStatus"/>
        /// </summary>
        /// <param name="connectionStatus">The new connection status of the user.</param>
        public void UpdateUserConnectionStatus(ConnectionStatus connectionStatus)
        {
            Contract.Requires(connectionStatus != null);
            
            User user = FindUserById(connectionStatus.UserId);

            User previousUser = User.DeepClone(user);

            user.ConnectionStatus = connectionStatus;

            EntityChangedEventArgs<User> userChangedEventArgs = new EntityChangedEventArgs<User>();

            userChangedEventArgs.EntityUpdated(user, previousUser);

            OnUserChanged(userChangedEventArgs);
        }

        /// <summary>
        /// Updates a <see cref="User"/>'s <see cref="Avatar"/>.
        /// </summary>
        /// <param name="avatar">The new avatar to give a user.</param>
        public void UpdateUserAvatar(Avatar avatar)
        {
            Contract.Requires(avatar != null);
            User user = FindUserById(avatar.UserId);

            User previousUser = User.DeepClone(user);

            user.Avatar = avatar;

            EntityChangedEventArgs<User> userChangedEventArgs = new EntityChangedEventArgs<User>();
            userChangedEventArgs.EntityUpdated(user, previousUser);

            OnUserChanged(userChangedEventArgs);
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
                usersIndexedById[user.Id] = user;
                Log.Debug("User with Id " + user.Id + " added to user repository");
            }
        }

        /// <summary>
        /// Retrieves a <see cref="User"/> entity from the repository.
        /// </summary>
        /// <param name="userId">The <see cref="User"/> entity ID to find.</param>
        /// <returns>The <see cref="User"/> which matches the ID. If no <see cref="User"/> is found, return null.</returns>
        public User FindUserById(int userId)
        {
            return usersIndexedById[userId];
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

        private void OnUserChanged(EntityChangedEventArgs<User> entityChangedEventArgs)
        {
            EventHandler<EntityChangedEventArgs<User>> userChangedCopy = UserChanged;

            if (userChangedCopy != null)
            {
                userChangedCopy(this, entityChangedEventArgs);
            }
        }
    }
}