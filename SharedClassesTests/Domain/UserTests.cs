using System;
using NUnit.Framework;
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
        public void CustomUserIDTest(int id)
        {
            var user = new User("user", id);
            Assert.AreEqual(user.UserId, id);
        }

        [Test]
        public void NoUniqueUserTest()
        {
            var userFactory = new UserIDGenerator();
            var user1 = new User("User1",userFactory.CreateUserId());

            Assert.AreEqual(user1.UserId, 0);

            var user2 = new User("User2", userFactory.CreateUserId());
            Assert.AreNotSame(user1.UserId, user2.UserId);
        }

        [Test]
        public void UserIDIterationTest()
        {
            var userFactory = new UserIDGenerator();
            User user = null;

            for (int i = 0; i <= 100; i++)
            {
                user = new User("User", userFactory.CreateUserId());
            }

            if (user != null)
            {
                Assert.AreEqual(user.UserId, 100);
            }
        }

        [Test]
        public void UserIDLowerThanZeroTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new User("user", -4));
        }

        [Test]
        public void UserWithSameIdentityChangesNameEqualityTest()
        {
            var user1 = new User("Tim", 1);
            var user2 = new User("Eric", 1);
            Assert.AreEqual(user1, user2);
        }

        [Test]
        public void UsersWithSameNameAndSameIDEqualityTest()
        {
            var user1 = new User("User", 2);
            var user2 = new User("User", 2);
            Assert.AreEqual(user1, user2);
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