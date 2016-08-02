using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Models a user in the system as an entity.
    /// </summary>
    [Serializable]
    public sealed class User : IEntity, IEquatable<User>
    {
        /// <summary>
        /// Creates an incomplete user entity.
        /// </summary>
        public User(string username)
        {
            Username = username;
            Avatar = new Avatar();
        }

        /// <summary>
        /// Creates a user entity with an Id.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="id">The unique Id of the user.</param>
        /// <param name="status">The current status of the user.</param>
        public User(string username, int id, ConnectionStatus status)
            : this(username)
        {
            Id = id;
            ConnectionStatus = status;
        }

        /// <summary>
        /// The name of the User.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// The user's current Avatar
        /// </summary>
        public Avatar Avatar { get; set; }

        /// <summary>
        /// The current status of the User.
        /// </summary>
        public ConnectionStatus ConnectionStatus { get; set; }

        /// <summary>
        /// A Unique number used to identify the User.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Deep clone a <see cref="User" /> entity.
        /// </summary>
        /// <param name="user">The user to deep clone.</param>
        /// <returns>The deep cloned user object.</returns>
        public static User DeepClone(User user)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, user);
                memoryStream.Position = 0;

                return (User) formatter.Deserialize(memoryStream);
            }
        }

        #region IEquality implementation

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is User && Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        #endregion
    }
}