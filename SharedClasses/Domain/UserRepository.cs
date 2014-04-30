using System.Collections.Generic;
using System.Linq;
using log4net;

namespace SharedClasses.Domain
{
    public class UserRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserRepository));

        private readonly Dictionary<int, User> usersIndexedById = new Dictionary<int, User>();

        public Dictionary<int, User> UsersIndexedById
        {
            get { return usersIndexedById; }
        }

        public void AddUser(User user)
        {
            usersIndexedById[user.UserId] = user;
            Log.Debug("User with Id " + user.UserId + " added to user repository");
        }

        public void AddUsers(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                usersIndexedById[user.UserId] = user;
                Log.Debug("User with Id " + user.UserId + " added to user repository");
            }
        }


        public void RemoveUser(int userId)
        {
            usersIndexedById.Remove(userId);
            Log.Debug("User with Id " + userId + " removed from user repository");
        }

        public IEnumerable<User> AllUsers()
        {
            List<User> users = usersIndexedById.Select(user => user.Value).ToList();
            return users;
        }

        public User FindUserByID(int userId)
        {
            return usersIndexedById[userId];
        }
    }
}