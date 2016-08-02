using System.Collections.Generic;
using NUnit.Framework;
using Server;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    public abstract class MessageHandlerTestFixture
    {
        protected int DefaultConversationIdDefaultUserIsIn;
        protected User DefaultUser;

        protected MockClientHandler ConnectedUserClientHandler { get; private set; }

        protected IServiceRegistry ServiceRegistry { get; private set; }

        [SetUp]
        public virtual void BeforeEachTest()
        {
            ServiceRegistry = new ServiceRegistry();

            var entityIdAllocatorFactory = new EntityIdAllocatorFactory();

            int userId = entityIdAllocatorFactory.AllocateEntityId<User>();
            DefaultUser = new User("user", userId, new ConnectionStatus(userId, ConnectionStatus.Status.Connected));

            ServiceRegistry.RegisterService<EntityIdAllocatorFactory>(entityIdAllocatorFactory);

            PopulateRepositoryManager(entityIdAllocatorFactory);
            PopulateClientManager();
        }

        private void PopulateClientManager()
        {
            IReadOnlyEntityRepository<User> userRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            var clientManager = new ClientManager();

            foreach (User user in userRepository.GetAllEntities())
            {
                if (user.Id == DefaultUser.Id)
                {
                    ConnectedUserClientHandler = new MockClientHandler();

                    clientManager.AddClientHandler(user.Id, ConnectedUserClientHandler);
                }
                else
                {
                    clientManager.AddClientHandler(user.Id, new MockClientHandler());
                }
            }

            ServiceRegistry.RegisterService<IClientManager>(clientManager);
        }

        private void PopulateRepositoryManager(EntityIdAllocatorFactory idAllocator)
        {
            var repositoryManager = new RepositoryManager();

            repositoryManager.AddRepository<User>(new UserRepository());
            repositoryManager.AddRepository<Conversation>(new ConversationRepository());
            repositoryManager.AddRepository<Participation>(new ParticipationRepository());

            ServiceRegistry.RegisterService<RepositoryManager>(repositoryManager);

            int userId2 = idAllocator.AllocateEntityId<User>();
            int userId3 = idAllocator.AllocateEntityId<User>();

            var usersToAddToConversation = new List<int> { DefaultUser.Id, userId2, userId3 };

            var userRepository = (UserRepository) repositoryManager.GetRepository<User>();
            var participationRepository = (ParticipationRepository) repositoryManager.GetRepository<Participation>();

            foreach (int userId in usersToAddToConversation)
            {
                var user = new User("user" + userId, userId, new ConnectionStatus(userId, ConnectionStatus.Status.Connected));
                userRepository.AddEntity(user);
            }

            SetUpMultiUserConversation(usersToAddToConversation, repositoryManager, idAllocator);

            DefaultConversationIdDefaultUserIsIn = participationRepository.GetConversationIdByUserIds(usersToAddToConversation);
        }

        private void SetUpMultiUserConversation(IEnumerable<int> userIds, RepositoryManager repositoryManager, EntityIdAllocatorFactory idAllocator)
        {
            var conversationRepository = (ConversationRepository) repositoryManager.GetRepository<Conversation>();
            var participationRepository = (ParticipationRepository) repositoryManager.GetRepository<Participation>();

            var conversation = new Conversation(idAllocator.AllocateEntityId<Conversation>());
            conversationRepository.AddEntity(conversation);

            foreach (int userId in userIds)
            {
                var participation = new Participation(idAllocator.AllocateEntityId<Participation>(), userId, conversation.Id);
                participationRepository.AddEntity(participation);
            }
        }

        public abstract void HandleMessage(IMessage message);
    }
}