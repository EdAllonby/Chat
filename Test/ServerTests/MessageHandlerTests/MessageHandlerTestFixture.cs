using NUnit.Framework;
using Server;
using SharedClasses;
using SharedClasses.Domain;

namespace ServerTests.MessageHandlerTests
{
    public class MessageHandlerTestFixture
    {
        protected const int ConnectedUserId = 1;
        private User userToDisconnect;

        protected IServiceRegistry ServiceRegistry { get; private set; }

        [SetUp]
        public virtual void BeforeEachTest()
        {
            userToDisconnect = new User("user", ConnectedUserId, new ConnectionStatus(ConnectedUserId, ConnectionStatus.Status.Connected));

            var clientManager = new ClientManager();
            var repositoryManager = new RepositoryManager();
            repositoryManager.UserRepository.AddEntity(userToDisconnect);
            clientManager.AddClientHandler(ConnectedUserId, new ClientHandler());

            ServiceRegistry = new ServiceRegistry();
            ServiceRegistry.RegisterService<IClientManager>(clientManager);
            ServiceRegistry.RegisterService<RepositoryManager>(repositoryManager);
            ServiceRegistry.RegisterService<EntityIdAllocatorFactory>(new EntityIdAllocatorFactory());
        }
    }
}