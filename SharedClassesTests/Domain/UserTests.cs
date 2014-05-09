using System;
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
        public void AssignCustomIDtoUserTest(int id)
        {
            var user = new User("user", id);
            Assert.AreEqual(user.UserId, id);
        }

        [TestCase("Tim", "Eric", 3)]
        [TestCase("Tim", "Tim", 3)]
        public void UsersWithSameIDEqualityTest(string user1Username, string user2Username, int userID)
        {
            var user1 = new User(user1Username, userID);
            var user2 = new User(user2Username, userID);

            Assert.AreEqual(user1, user2);
        }

        [Test]
        public void IDEqualityTest()
        {
            var userFactory = new EntityIDGenerator();
            var user1 = new User("User1", userFactory.AssignEntityID());

            Assert.AreEqual(user1.UserId, 0);

            var user2 = new User("User2", userFactory.AssignEntityID());
            Assert.AreNotSame(user1.UserId, user2.UserId);
        }

        [Test]
        public void UserIDIterationTest()
        {
            var userFactory = new EntityIDGenerator();
            User user = null;

            for (int i = 0; i <= 100; i++)
            {
                user = new User("User", userFactory.AssignEntityID());
            }

            if (user != null)
            {
                Assert.AreEqual(user.UserId, 100);
            }
        }

        [Test]
        public void UserIDLowerThanZeroThrowsExceptionTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new User("user", -4));
        }

        [Test]
        public void UsersWithSameNameHaveDifferentIDsEqualityTest()
        {
            var user1 = new User("User", 1);
            var user2 = new User("User", 2);
            Assert.AreNotEqual(user1, user2);
        }
    }
}