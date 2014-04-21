using SharedClasses.Domain;

namespace SharedClasses
{
    public sealed class UserIDGenerator
    {
        public int NextID { get; private set; }

        public User CreateUser(string username)
        {
            var user = new User(username, NextID);
            NextID++;
            return user;
        }
    }
}