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
                IEntityRepository userRepository = new UserRepository();
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
                IEntityRepository userRepository = new UserRepository();
                IEntityRepository conversationRepository = new ConversationRepository();
                IEntityRepository participationRepository = new ParticipationRepository();

                var repositoryManager = new RepositoryManager();
                repositoryManager.AddRepository<User>(userRepository);
                repositoryManager.AddRepository<Conversation>(conversationRepository);
                repositoryManager.AddRepository<Participation>(participationRepository);

                IReadOnlyEntityRepository<Conversation> retrievedConversationRepository = repositoryManager.GetRepository<Conversation>();
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