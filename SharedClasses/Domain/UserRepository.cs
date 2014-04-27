using System.Collections.Generic;

namespace SharedClasses.Domain
{
    public class UserRepository
    {
        private readonly Dictionary<int, User> usersIndexedById = new Dictionary<int, User>();

        public void AddUser(User user)
        {
            usersIndexedById[user.UserId] = user;
        }

        public void RemoveUser(int userId)
        {
            // Remove user;
        }

        public Dictionary<int, User> UsersIndexedById
        {
            get
            {
                return usersIndexedById;
            }
        }

        public User FindUserByID(int userId)
        {
            return usersIndexedById[userId];
        }
    }
}