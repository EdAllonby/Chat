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
        public void AssignCustomIDtoUserTest(int id)
        {
            var user = new User("user", id, ConnectionStatus.Connected);
            Assert.AreEqual(user.UserId, id);
        }

        [TestCase("Tim", "Eric", 3)]
        [TestCase("Tim", "Tim", 3)]
        public void UsersWithSameIDEqualityTest(string firstUserUsername, string secondUserUsername, int userID)
        {
            var user1 = new User(firstUserUsername, userID, ConnectionStatus.Connected);
            var user2 = new User(secondUserUsername, userID, ConnectionStatus.Connected);

            Assert.AreEqual(user1, user2);
        }

        [TestCase(7)]
        [TestCase(123987)]
        public void UserIDIterationTest(int userCount)
        {
            int totalUsers = userCount;

            var entityGeneratorFactory = new EntityGeneratorFactory();

            User user = null;

            for (int i = 0; i < totalUsers; i++)
            {
                user = new User("User", entityGeneratorFactory.GetEntityID<User>(), ConnectionStatus.Connected);
            }

            Contract.Assert(user != null, "user != null");
            Assert.AreEqual(user.UserId, totalUsers);
        }

        [Test]
        public void UserEqualityTest()
        {
            var entityGeneratorFactory = new EntityGeneratorFactory();

            var user1 = new User("User1", entityGeneratorFactory.GetEntityID<User>(), ConnectionStatus.Connected);

            Assert.AreEqual(user1.UserId, 1);

            var user2 = new User("User2", entityGeneratorFactory.GetEntityID<User>(), ConnectionStatus.Connected);
            Assert.AreNotSame(user1.UserId, user2.UserId);

            Assert.IsFalse(user1.Equals(user2 as object));
        }

        [Test]
        public void UsersWithSameNameHaveDifferentIDsEqualityTest()
        {
            var user1 = new User("User", 1, ConnectionStatus.Connected);
            var user2 = new User("User", 2, ConnectionStatus.Connected);
            Assert.AreNotEqual(user1, user2);
        }

        [Test]
        public void HashcodeTest()
        {
            var user1 = new User("User", 1, ConnectionStatus.Connected);
            var user2 = new User("User", 1, ConnectionStatus.Connected);

            Assert.AreEqual(user1.GetHashCode(), user2.GetHashCode());
        }

        [Test]
        public void ReferenceEqualsTest()
        {
            var user1 = new User("User", 2, ConnectionStatus.Disconnected);
            var user2 = user1;

            Assert.IsTrue(user1.Equals(user2));
            Assert.IsTrue(user1.Equals(user2 as object));
            Assert.IsFalse(user1.Equals(null));

            object userObject = user1;

            Assert.IsFalse(userObject.Equals(2));
            Assert.IsFalse(userObject.Equals(null));
        }
    }
}