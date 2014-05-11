using System.Collections.Generic;
using System.Linq;
using log4net;

namespace SharedClasses.Domain
{
    internal sealed class UserRepository : IEntityRepository<User>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (UserRepository));

        private readonly Dictionary<int, User> usersIndexedById = new Dictionary<int, User>();

        public void AddEntity(User user)
        {
            usersIndexedById[user.UserId] = user;
            Log.Debug("User with Id " + user.UserId + " added to user repository");
        }

        public void AddEntities(IEnumerable<User> users)
        {
            foreach (User user in users)
            {
                usersIndexedById[user.UserId] = user;
                Log.Debug("User with Id " + user.UserId + " added to user repository");
            }
        }

        public void RemoveEntity(int userId)
        {
            usersIndexedById.Remove(userId);
            Log.Debug("User with Id " + userId + " removed from user repository");
        }

        public User FindEntityByID(int userId)
        {
            return usersIndexedById[userId];
        }

        public IEnumerable<User> GetAllEntities()
        {
            return usersIndexedById.Values.ToList();
        }
    }
}