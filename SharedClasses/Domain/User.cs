using System;
using System.Diagnostics.Contracts;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Models a user in the system as an entity.
    /// </summary>
    [Serializable]
    public sealed class User : IEquatable<User>
    {
        private readonly int userId;
        private readonly string username;

        /// <summary>
        /// Creates an incomplete user entity.
        /// </summary>
        public User(string username)
        {
            Contract.Requires(username != null);

            this.username = username;
        }

        /// <summary>
        /// Creates a user entity with an Id.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="userId">The unique Id of the user.</param>
        /// <param name="status">The current status of the user.</param>
        public User(string username, int userId, ConnectionStatus status)
            : this(username)
        {
            Contract.Requires(username != null);
            Contract.Requires(userId > 0);

            this.userId = userId;
            ConnectionStatus = status;
        }

        /// <summary>
        /// A Unique number used to identify the User.
        /// </summary>
        public int UserId
        {
            get { return userId; }
        }

        /// <summary>
        /// The name of the User.
        /// </summary>
        public string Username
        {
            get { return username; }
        }

        /// <summary>
        /// The current status of the User.
        /// </summary>
        public ConnectionStatus ConnectionStatus { get; set; }

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

            return userId == other.userId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is User && Equals((User) obj);
        }

        public override int GetHashCode()
        {
            return userId;
        }
    }
}