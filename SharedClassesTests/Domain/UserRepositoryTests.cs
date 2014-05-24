using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharedClasses;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class UserRepositoryTests
    {
        [Test]
        public void AddUserEntitiesTest()
        {
            var user1 = new User("User", 1, ConnectionStatus.Connected);
            var user2 = new User("User", 2, ConnectionStatus.Connected);
            var users = new List<User> {user1, user2};

            var userRepository = new UserRepository();

            userRepository.AddUsers(users);
            Assert.AreEqual(users, userRepository.GetAllUsers().ToList());
        }

        [Test]
        public void AddAndUpdateUserEntityTest()
        {
            var user = new User("User", 2, ConnectionStatus.Connected);
            var userRepository = new UserRepository();
            userRepository.AddUser(user);

            Assert.AreEqual(user, userRepository.FindUserByID(user.UserId));

            user.ConnectionStatus = ConnectionStatus.Disconnected;
            userRepository.AddUser(user);

            Assert.AreEqual(user, userRepository.FindUserByID(user.UserId));
        }

        [Test]
        public void CanNotAddSameUserEntityTwice()
        {
            var user = new User("User", 1, ConnectionStatus.Connected);

            var userRepository = new UserRepository();

            userRepository.AddUser(user);
            userRepository.AddUser(user);

            var users = new List<User> {user};

            Assert.AreEqual(users, userRepository.GetAllUsers());
        }

        [Test]
        public void FindNonExistentUserTest()
        {
            var userRepository = new UserRepository();

            Assert.IsNull(userRepository.FindUserByID(3));
        }

        [Test]
        public void FindUserByUsernameTest()
        {
            var userRepository = new UserRepository();

            string username = "User";
            var user = new User(username, 3, ConnectionStatus.Connected);
            userRepository.AddUser(user);

            Assert.AreEqual(user, userRepository.FindUserByUsername(username));

            Assert.IsNull(userRepository.FindUserByUsername("Anon"));
        }
    }
}