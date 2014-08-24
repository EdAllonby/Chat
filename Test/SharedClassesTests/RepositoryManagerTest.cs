using NUnit.Framework;
using SharedClasses;
using SharedClasses.Domain;

namespace SharedClassesTests
{
    [TestFixture]
    public class RepositoryManagerTest
    {
        [TestFixture]
        public class AddRepositoryTest : RepositoryManagerTest
        {
            [Test]
            public void CanAddRepository()
            {
                IRepository userRepository = new UserRepository();
                var repositoryManager = new RepositoryManager();
                repositoryManager.AddRepository<User>(userRepository);
                Assert.AreEqual(userRepository, repositoryManager.GetRepository<User>());
            }
        }

        [TestFixture]
        public class GetRepositoryTest : RepositoryManagerTest
        {
            [Test]
            public void ReturnsCorrectRepository()
            {
                IRepository userRepository = new UserRepository();
                IRepository conversationRepository = new ConversationRepository();
                IRepository participationRepository = new ParticipationRepository();

                var repositoryManager = new RepositoryManager();
                repositoryManager.AddRepository<User>(userRepository);
                repositoryManager.AddRepository<Conversation>(conversationRepository);
                repositoryManager.AddRepository<Participation>(participationRepository);

                IReadOnlyRepository<Conversation> retrievedConversationRepository = repositoryManager.GetRepository<Conversation>();
                Assert.AreEqual(conversationRepository, retrievedConversationRepository);
            }

            [Test]
            public void ReturnsNullIfNoRepositoryIsFound()
            {
                var repositoryManager = new RepositoryManager();
                Assert.IsNull(repositoryManager.GetRepository<Participation>());
            }
        }
    }
}