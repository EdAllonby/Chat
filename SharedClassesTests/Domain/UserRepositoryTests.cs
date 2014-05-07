using System.Collections.Generic;
using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private UserRepository userRepository;
        private User user0;
        private User user1;
        private User user2;

        [TestFixtureSetUp]
        public void CreateRepository()
        {
            userRepository = new UserRepository();

            user0 = new User("User0", 0);
            user1 = new User("User1", 1);
            user2 = new User("User2", 2);
        }

        [Test]
        public void AddUserTest()
        {
            userRepository.AddUser(user1);
            Assert.AreSame(user1, userRepository.UsersIndexedById[user1.UserId]);
        }

        [Test]
        public void AddUsersTest()
        {
            var users = new List<User> {user1, user2};
            userRepository.AddUsers(users);
            Assert.AreSame(user1, userRepository.UsersIndexedById[user1.UserId]);
            Assert.AreSame(user2, userRepository.UsersIndexedById[user2.UserId]);
        }

        [Test]
        public void CanNotAddSameUserTwice()
        {
            userRepository.AddUser(user0);
            userRepository.AddUser(user0);

            IEnumerable<User> users = new List<User> {user0};

            IEnumerable<User> userCollection = userRepository.RetrieveAllUsers();
            Assert.AreEqual(users, userCollection);
        }

        [Test]
        public void FindUserByIdTest()
        {
            userRepository.AddUser(user0);
            User retrievedUser = userRepository.FindUserById(user0.UserId);
            Assert.AreEqual(user0, retrievedUser);
        }

        [Test]
        public void RemoveUserTest()
        {
            userRepository.RemoveUser(user0.UserId);
            Assert.IsEmpty(userRepository.UsersIndexedById.Values);
        }

        [Test]
        public void RetrieveAllUsersTest()
        {
            IEnumerable<User> users = new List<User> {user0, user1, user2};

            userRepository.AddUsers(users);

            IEnumerable<User> userCollection = userRepository.RetrieveAllUsers();
            Assert.AreEqual(users, userCollection);
        }
    }
}