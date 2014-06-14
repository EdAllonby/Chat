using System.Diagnostics.Contracts;
using NUnit.Framework;
using Server;
using SharedClasses;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class UserTests
    {
        [TestCase(2)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(8)]
        public void AssignCustomIdtoUserTest(int userId)
        {
            var user = new User("user", userId, ConnectionStatus.Connected);
            Assert.AreEqual(user.UserId, userId);
        }

        [TestCase("Tim", "Eric", 3)]
        [TestCase("Tim", "Tim", 3)]
        public void UsersWithSameIdEqualityTest(string firstUserUsername, string secondUserUsername, int userID)
        {
            var user1 = new User(firstUserUsername, userID, ConnectionStatus.Connected);
            var user2 = new User(secondUserUsername, userID, ConnectionStatus.Connected);

            Assert.AreEqual(user1, user2);
        }

        [TestCase(100)]
        [TestCase(1291)]
        public void UserIdIterationTest(int userCount)
        {
            EntityGeneratorFactory entityGenerator = new EntityGeneratorFactory();

            int baseId = entityGenerator.GetEntityID<User>();

            int totalUsers = userCount;

            User user = null;

            for (int i = 0; i < totalUsers; i++)
            {
                user = new User("User", entityGenerator.GetEntityID<User>(), ConnectionStatus.Connected);
            }

            Contract.Assert(user != null, "user != null");
            Assert.AreEqual(user.UserId, totalUsers + baseId);
        }

        [Test]
        public void UserEqualsTest()
        {
            EntityGeneratorFactory entityGenerator = new EntityGeneratorFactory();

            int user1EntityId = entityGenerator.GetEntityID<User>();

            var user1 = new User("User1", user1EntityId, ConnectionStatus.Connected);

            Assert.AreEqual(user1.UserId, user1EntityId);

            var user2 = new User("User2", entityGenerator.GetEntityID<User>(), ConnectionStatus.Connected);
            Assert.AreNotSame(user1.UserId, user2.UserId);

            Assert.IsFalse(user1.Equals(user2 as object));
        }

        [Test]
        public void UserHashCodeTest()
        {
            var user1 = new User("User", 1, ConnectionStatus.Connected);
            var user2 = new User("User", 1, ConnectionStatus.Connected);

            Assert.AreEqual(user1.GetHashCode(), user2.GetHashCode());
        }

        [Test]
        public void UserReferenceEqualsTest()
        {
            var user1 = new User("User", 2, ConnectionStatus.Disconnected);
            User user2 = user1;

            Assert.IsTrue(user1.Equals(user2));
            Assert.IsTrue(user1.Equals(user2 as object));
            Assert.IsFalse(user1.Equals(null));

            object userObject = user1;

            Assert.IsFalse(userObject.Equals(2));
            Assert.IsFalse(userObject.Equals(null));
        }

        [Test]
        public void UsersWithSameNameHaveDifferentIDsEqualityTest()
        {
            var user1 = new User("User", 1, ConnectionStatus.Connected);
            var user2 = new User("User", 2, ConnectionStatus.Connected);
            Assert.AreNotEqual(user1, user2);
        }
    }
}