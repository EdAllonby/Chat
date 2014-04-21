using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Models a user in the system.
    /// </summary>
    [Serializable]
    public sealed class User : IEquatable<User>
    {
        private readonly int id;

        public User(string username, int id)
        {
            UserName = username;

            if (id < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.id = id;
        }

        /// <summary>
        /// The name of the User
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// A Unique number used to identify the User.
        /// </summary>
        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// Find a User in a List of Users by its ID.
        /// </summary>
        /// <param name="connectedUserCollection">The collection of Users that will be searched</param>
        /// <param name="userID">The User ID to find in the User collection</param>
        /// <returns>The User that matches the User ID</returns>
        public static User FindByUserID(IEnumerable<User> connectedUserCollection, int userID)
        {
            return connectedUserCollection.FirstOrDefault(s => s.ID.Equals(userID));
        }

        #region IEquatable Implementation

        public bool Equals(User other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return id == other.id && string.Equals(UserName, other.UserName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is User && Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (id*397) ^ (UserName != null ? UserName.GetHashCode() : 0);
            }
        }

        #endregion
    }
}