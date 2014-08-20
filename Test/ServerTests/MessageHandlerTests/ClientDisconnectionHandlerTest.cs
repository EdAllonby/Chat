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
    public class ClientDisconnectionHandlerTest : MessageHandlerTestFixture
    {
        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            clientDisconnection = new ClientDisconnection(ConnectedUserId);
        }

        private ClientDisconnection clientDisconnection;
        private readonly ClientDisconnectionHandler clientDisconnectionHandler = new ClientDisconnectionHandler();

        [TestFixture]
        public class HandleMessageTest : ClientDisconnectionHandlerTest
        {
            [Test]
            public void ClientGetsRemovesFromClientHandler()
            {
                clientDisconnectionHandler.HandleMessage(clientDisconnection, ServiceRegistry);
                Assert.IsFalse(ServiceRegistry.GetService<IClientManager>().HasClientHandler(ConnectedUserId));
            }

            [Test]
            public void RepositoryUpdatesUser()
            {
                bool isUserUpdated = false;
                ServiceRegistry.GetService<RepositoryManager>().UserRepository.EntityUpdated += (sender, eventArgs) => isUserUpdated = true;
                clientDisconnectionHandler.HandleMessage(clientDisconnection, ServiceRegistry);
                Assert.IsTrue(isUserUpdated);
            }

            [Test]
            public void ThrowsExceptionWhenNotGivenClientDisconnection()
            {
                Assert.Throws<InvalidCastException>(() => clientDisconnectionHandler.HandleMessage(new LoginRequest("user"), ServiceRegistry));
            }

            [Test]
            public void UserGetsSetToDisconnectedInUserRepository()
            {
                clientDisconnectionHandler.HandleMessage(clientDisconnection, ServiceRegistry);
                Assert.IsTrue(ServiceRegistry.GetService<RepositoryManager>().UserRepository.FindEntityById(ConnectedUserId).ConnectionStatus.UserConnectionStatus.Equals(ConnectionStatus.Status.Disconnected));
            }
        }
    }
}