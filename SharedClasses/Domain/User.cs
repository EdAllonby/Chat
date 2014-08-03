using System;
using System.Diagnostics.Contracts;
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
        private readonly int id;
        private readonly string username;
        private Avatar avatar;

        /// <summary>
        /// Creates an incomplete user entity.
        /// </summary>
        public User(string username)
        {
            Contract.Requires(username != null);

            this.username = username;
            avatar = new Avatar();
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
            Contract.Requires(username != null);
            Contract.Requires(id > 0);

            this.id = id;
            ConnectionStatus = status;
        }

        /// <summary>
        /// The name of the User.
        /// </summary>
        public string Username
        {
            get { return username; }
        }

        /// <summary>
        /// The user's current Avatar
        /// </summary>
        public Avatar Avatar
        {
            get { return avatar; }
            set { avatar = value; }
        }

        /// <summary>
        /// The current status of the User.
        /// </summary>
        public ConnectionStatus ConnectionStatus { get; set; }

        /// <summary>
        /// A Unique number used to identify the User.
        /// </summary>
        public int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Deep clone a <see cref="User"/> entity.
        /// </summary>
        /// <param name="user">The user to deep clone.</param>
        /// <returns>The deep cloned user object.</returns>
        public static User DeepClone(User user)
        {
            Contract.Requires(user != null);

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

            return id == other.id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is User && Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return id;
        }

        #endregion
    }
}