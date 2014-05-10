using System.Collections.Generic;
using NUnit.Framework;
using SharedClasses;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private IEntityRepository<User> userRepository;
        private RepositoryFactory repositoryFactory;


        private User user0;
        private User user1;
        private User user2;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            user0 = new User("User0", 1);
            user1 = new User("User1", 2);
            user2 = new User("User2", 3);
        }

        [Test]
        public void AddUserTest()
        {
            repositoryFactory = new RepositoryFactory();
            userRepository = repositoryFactory.GetRepository<User>();

            userRepository.AddEntity(user1);
            Assert.AreSame(user1, userRepository.FindEntityByID(user1.UserId));
        }

        [Test]
        public void AddUsersTest()
        {
            repositoryFactory = new RepositoryFactory();
            userRepository = repositoryFactory.GetRepository<User>();

            var users = new List<User> {user1, user2};
            userRepository.AddEntities(users);
            Assert.AreSame(user1, userRepository.FindEntityByID(user1.UserId));
            Assert.AreSame(user2, userRepository.FindEntityByID(user2.UserId));
        }

        [Test]
        public void CanNotAddSameUserTwice()
        {
            repositoryFactory = new RepositoryFactory();
            userRepository = repositoryFactory.GetRepository<User>();

            userRepository.AddEntity(user0);
            userRepository.AddEntity(user0);

            var users = new List<User> {user0};

            IEnumerable<User> userCollection = userRepository.GetAllEntities();
            Assert.AreEqual(users, userCollection);
        }

        [Test]
        public void FindUserByIdTest()
        {
            repositoryFactory = new RepositoryFactory();
            userRepository = repositoryFactory.GetRepository<User>();

            userRepository.AddEntity(user0);
            User retrievedUser = userRepository.FindEntityByID(user0.UserId);
            Assert.AreEqual(user0, retrievedUser);
        }

        [Test]
        public void RemoveUserTest()
        {
            repositoryFactory = new RepositoryFactory();
            userRepository = repositoryFactory.GetRepository<User>();

            userRepository.RemoveEntity(user0.UserId);
            Assert.IsEmpty(userRepository.GetAllEntities());
        }

        [Test]
        public void RetrieveAllUsersTest()
        {
            repositoryFactory = new RepositoryFactory();
            userRepository = repositoryFactory.GetRepository<User>();

            IEnumerable<User> users = new List<User> {user0, user1, user2};

            userRepository.AddEntities(users);

            IEnumerable<User> userCollection = userRepository.GetAllEntities();
            Assert.AreEqual(users, userCollection);
        }
    }
}