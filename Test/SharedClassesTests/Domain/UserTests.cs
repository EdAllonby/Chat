using NUnit.Framework;
using Server;
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
            var user = new User("user", userId, new ConnectionStatus(userId, ConnectionStatus.Status.Connected));
            Assert.AreEqual(user.Id, userId);
        }

        [TestCase("Tim", "Eric", 3)]
        [TestCase("Tim", "Tim", 3)]
        public void UsersWithSameIdEqualityTest(string firstUserUsername, string secondUserUsername, int userId)
        {
            var user1 = new User(firstUserUsername, userId, new ConnectionStatus(userId, ConnectionStatus.Status.Connected));
            var user2 = new User(secondUserUsername, userId, new ConnectionStatus(userId, ConnectionStatus.Status.Connected));

            Assert.AreEqual(user1, user2);
        }

        [TestCase(100)]
        [TestCase(1291)]
        public void UserIdIterationTest(int userCount)
        {
            var entityGenerator = new EntityIdAllocatorFactory();

            int baseId = entityGenerator.AllocateEntityId<User>();

            int totalUsers = userCount;

            User user = null;

            for (var i = 0; i < totalUsers; i++)
            {
                int userId = entityGenerator.AllocateEntityId<User>();
                user = new User("User", userId, new ConnectionStatus(userId, ConnectionStatus.Status.Connected));
            }

            Assert.AreEqual(user.Id, totalUsers + baseId);
        }

        [Test]
        public void UserEqualsTest()
        {
            var entityGenerator = new EntityIdAllocatorFactory();

            int user1EntityId = entityGenerator.AllocateEntityId<User>();

            var user1 = new User("User1", user1EntityId, new ConnectionStatus(user1EntityId, ConnectionStatus.Status.Connected));

            Assert.AreEqual(user1.Id, user1EntityId);

            int user2EntityId = entityGenerator.AllocateEntityId<User>();

            var user2 = new User("User2", user2EntityId, new ConnectionStatus(user2EntityId, ConnectionStatus.Status.Connected));
            Assert.AreNotSame(user1.Id, user2.Id);

            Assert.IsFalse(user1.Equals(user2 as object));
        }

        [Test]
        public void UserHashCodeTest()
        {
            var user1 = new User("User", 1, new ConnectionStatus(1, ConnectionStatus.Status.Connected));
            var user2 = new User("User", 1, new ConnectionStatus(1, ConnectionStatus.Status.Connected));

            Assert.AreEqual(user1.GetHashCode(), user2.GetHashCode());
        }

        [Test]
        public void UserReferenceEqualsTest()
        {
            var user1 = new User("User", 1, new ConnectionStatus(1, ConnectionStatus.Status.Connected));
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
            var user1 = new User("User", 1, new ConnectionStatus(1, ConnectionStatus.Status.Connected));
            var user2 = new User("User", 2, new ConnectionStatus(2, ConnectionStatus.Status.Connected));
            Assert.AreNotEqual(user1, user2);
        }
    }
}