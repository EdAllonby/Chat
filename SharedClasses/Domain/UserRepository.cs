using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Holds a collection of <see cref="User"/>s with basic CRUD operations.
    /// </summary>
    public sealed class UserRepository : Repository<User>
    {
        /// <summary>
        /// Updates a <see cref="User"/>'s <see cref="ConnectionStatus"/>
        /// </summary>
        /// <param name="connectionStatus">The new connection status of the user.</param>
        public void UpdateUserConnectionStatus(ConnectionStatus connectionStatus)
        {
            Contract.Requires(connectionStatus != null);
            
            User user = FindEntityById(connectionStatus.UserId);

            User previousUser = User.DeepClone(user);

            user.ConnectionStatus = connectionStatus;

            var userChangedEventArgs = new EntityChangedEventArgs<User>();

            userChangedEventArgs.EntityUpdated(user, previousUser);

            OnEntityChanged(userChangedEventArgs);
        }

        /// <summary>
        /// Updates a <see cref="User"/>'s <see cref="Avatar"/>.
        /// </summary>
        /// <param name="avatar">The new avatar to give a user.</param>
        public void UpdateUserAvatar(Avatar avatar)
        {
            Contract.Requires(avatar != null);
            User user = FindEntityById(avatar.UserId);

            User previousUser = User.DeepClone(user);

            user.Avatar = avatar;

            var userChangedEventArgs = new EntityChangedEventArgs<User>();
            userChangedEventArgs.EntityUpdated(user, previousUser);

            OnEntityChanged(userChangedEventArgs);
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
                EntitiesIndexedById[user.Id] = user;
                Log.Debug("User with Id " + user.Id + " added to user repository");
            }
        }

        /// <summary>
        /// Gets a <see cref="User"/> entity by username.
        /// </summary>
        /// <param name="username">The username that is used to find the <see cref="User"/>.</param>
        /// <returns>The <see cref="User"/> that matches the username.</returns>
        public User FindUserByUsername(string username)
        {
            return EntitiesIndexedById.Where(user => user.Value.Username == username).Select(user => user.Value).FirstOrDefault();
        }
    }
}