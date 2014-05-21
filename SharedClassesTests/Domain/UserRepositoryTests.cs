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
            Assert.AreEqual(users, userRepository.GetAllEntities().ToList());
        }

        [Test]
        public void AddUserEntityTest()
        {
            var user = new User("User", 2, ConnectionStatus.Connected);
            var userRepository = new UserRepository();
            userRepository.AddEntity(user);
            Assert.AreEqual(user, userRepository.FindEntityByID(user.UserId));
        }

        [Test]
        public void CanNotAddSameUserEntityTwice()
        {
            var user = new User("User", 1, ConnectionStatus.Connected);

            var userRepository = new UserRepository();

            userRepository.AddEntity(user);
            userRepository.AddEntity(user);

            var users = new List<User> {user};

            Assert.AreEqual(users, userRepository.GetAllEntities());
        }
    }
}