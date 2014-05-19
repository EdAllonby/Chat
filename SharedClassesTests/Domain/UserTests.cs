﻿using NUnit.Framework;
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
        public void UsersWithSameIDEqualityTest(string firstUserUsername, string secondUserUsername, int userID)
        {
            var user1 = new User(firstUserUsername, userID);
            var user2 = new User(secondUserUsername, userID);

            Assert.AreEqual(user1, user2);
        }

        [Test]
        public void IDEqualityTest()
        {
            EntityGeneratorFactory entityGeneratorFactory = new EntityGeneratorFactory();

            var user1 = new User("User1", entityGeneratorFactory.GetEntityID<User>());

            Assert.AreEqual(user1.UserId, 1);

            var user2 = new User("User2", entityGeneratorFactory.GetEntityID<User>());
            Assert.AreNotSame(user1.UserId, user2.UserId);
        }

        [TestCase(7)]
        [TestCase(123987)]
        public void UserIDIterationTest(int userCount)
        {
            int totalUsers = userCount;

            EntityGeneratorFactory entityGeneratorFactory = new EntityGeneratorFactory();

            User user = null;

            for (int i = 0; i < totalUsers; i++)
            {
                user = new User("User", entityGeneratorFactory.GetEntityID<User>());
            }

            if (user != null)
            {
                Assert.AreEqual(user.UserId, totalUsers);
            }
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