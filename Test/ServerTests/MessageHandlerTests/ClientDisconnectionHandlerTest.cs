using System;
using NUnit.Framework;
using Server;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class ClientDisconnectionHandlerTest
    {
        private const int UserId = 1;
        private ClientDisconnection clientDisconnection;
        private ClientDisconnectionHandler clientDisconnectionHandler;
        private IServerMessageContext serverMessageContext;
        private User userToDisconnect;

        [SetUp]
        public void BeforeEachTest()
        {
            clientDisconnection = new ClientDisconnection(UserId);
            clientDisconnectionHandler = new ClientDisconnectionHandler();
            userToDisconnect = new User("user", UserId, new ConnectionStatus(UserId, ConnectionStatus.Status.Connected));
            ClientManager clientManager = new ClientManager();
            RepositoryManager repositoryManager = new RepositoryManager();
            repositoryManager.UserRepository.AddEntity(userToDisconnect);
            clientManager.AddClientHandler(1, new ClientHandler());
            serverMessageContext = new ServerMessageContext(clientManager, new EntityIdAllocatorFactory(), repositoryManager);
        }

        [TestFixture]
        public class HandleMessageTest : ClientDisconnectionHandlerTest
        {
            [Test]
            public void ClientGetsRemovesFromClientHandler()
            {
                clientDisconnectionHandler.HandleMessage(clientDisconnection, serverMessageContext);
                Assert.IsFalse(serverMessageContext.ClientManager.HasClientHandler(UserId));
            }

            [Test]
            public void UserGetsSetToDisconnectedInUserRepository()
            {
                clientDisconnectionHandler.HandleMessage(clientDisconnection, serverMessageContext);
                Assert.IsTrue(serverMessageContext.RepositoryManager.UserRepository.FindEntityById(UserId).ConnectionStatus.UserConnectionStatus.Equals(ConnectionStatus.Status.Disconnected));
            }

            [Test]
            public void RepositoryUpdatesUser()
            {
                bool isUserUpdated = false;
                serverMessageContext.RepositoryManager.UserRepository.EntityUpdated += (sender, eventArgs) => isUserUpdated = true;
                clientDisconnectionHandler.HandleMessage(clientDisconnection, serverMessageContext);
                Assert.IsTrue(isUserUpdated);
            }

            [Test]
            public void ThrowsExceptionWhenNotGivenClientDisconnection()
            {
                Assert.Throws<InvalidCastException>(() => clientDisconnectionHandler.HandleMessage(new LoginRequest("user"), serverMessageContext));
            }
        }
    }
}