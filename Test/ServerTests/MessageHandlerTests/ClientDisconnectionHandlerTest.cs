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

            clientDisconnection = new ClientDisconnection(DefaultUser.Id);
        }

        public override void HandleMessage(IMessage message)
        {
            clientDisconnectionHandler.HandleMessage(message, ServiceRegistry);
        }

        private ClientDisconnection clientDisconnection;
        private readonly ClientDisconnectionHandler clientDisconnectionHandler = new ClientDisconnectionHandler();

        [TestFixture]
        public class HandleMessageTest : ClientDisconnectionHandlerTest
        {
            [Test]
            public void ClientHandlerGetsRemovedFromClientManager()
            {
                HandleMessage(clientDisconnection);
                Assert.IsFalse(ServiceRegistry.GetService<IClientManager>().HasClientHandler(DefaultUser.Id));
            }

            [Test]
            public void RepositoryUpdatesUser()
            {
                bool isUserUpdated = false;
                ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>().EntityUpdated += (sender, eventArgs) => isUserUpdated = true;
                HandleMessage(clientDisconnection);
                Assert.IsTrue(isUserUpdated);
            }

            [Test]
            public void ThrowsExceptionWhenNotGivenClientDisconnection()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new LoginRequest("user")));
            }

            [Test]
            public void UserGetsSetToDisconnectedInUserRepository()
            {
                HandleMessage(clientDisconnection);
                Assert.IsTrue(ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>().FindEntityById(DefaultUser.Id).ConnectionStatus.UserConnectionStatus.Equals(ConnectionStatus.Status.Disconnected));
            }
        }
    }
}