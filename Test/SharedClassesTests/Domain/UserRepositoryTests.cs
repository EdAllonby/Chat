using System;
using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class UserRepositoryTests
    {
        [Test]
        public void AddAndUpdateUserEntityTest()
        {
            var user = new User("User", 2, new ConnectionStatus(2, ConnectionStatus.Status.Connected));
            var userRepository = new UserRepository();
            userRepository.AddEntity(user);

            Assert.AreEqual(user, userRepository.FindEntityById(user.Id));

            user.ConnectionStatus = new ConnectionStatus(2, ConnectionStatus.Status.Connected);

            userRepository.UpdateUserConnectionStatus(new ConnectionStatus(user.Id, ConnectionStatus.Status.Connected));

            Assert.AreEqual(user, userRepository.FindEntityById(user.Id));
        }

        [Test]
        public void CanNotAddSameUserEntityTwice()
        {
            var user = new User("User", 1, new ConnectionStatus(1, ConnectionStatus.Status.Connected));

            var userRepository = new UserRepository();

            userRepository.AddEntity(user);

            Assert.Throws<ArgumentException>(() => userRepository.AddEntity(user));
        }

        [Test]
        public void FindUserByUsernameTest()
        {
            var userRepository = new UserRepository();

            const string Username = "User";
            var user = new User(Username, 3, new ConnectionStatus(3, ConnectionStatus.Status.Connected));

            userRepository.AddEntity(user);

            Assert.AreEqual(user, userRepository.FindUserByUsername(Username));

            Assert.IsNull(userRepository.FindUserByUsername("Anon"));
        }
    }
}