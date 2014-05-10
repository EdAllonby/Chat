using System;
using System.Diagnostics.Contracts;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Models a user in the system as an entity class (has identity)
    /// </summary>
    [Serializable]
    public sealed class User : IEquatable<User>
    {
        // User should be immutable, once made it will never change
        private readonly int userId;
        private readonly string username;

        public User(string username, int userId)
        {
            Contract.Requires(username != null);
            Contract.Requires(userId > 0);

            this.username = username;
            this.userId = userId;
        }

        /// <summary>
        /// Incomplete user
        /// </summary>
        public User(string username)
        {
            this.username = username;
        }

        /// <summary>
        /// The name of the User
        /// </summary>
        public string Username
        {
            get { return username; }
        }

        /// <summary>
        /// A Unique number used to identify the User.
        /// </summary>
        public int UserId
        {
            get { return userId; }
        }

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