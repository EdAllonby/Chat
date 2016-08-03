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
            var clientDisconnectionHandler = new ClientDisconnectionHandler(ServiceRegistry);
            clientDisconnectionHandler.HandleMessage(message);
        }

        private ClientDisconnection clientDisconnection;

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
                var isUserUpdated = false;
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
                IReadOnlyEntityRepository<User> userRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();
                User defaultUser = userRepository.FindEntityById(DefaultUser.Id);
                Assert.IsTrue(defaultUser.ConnectionStatus.UserConnectionStatus.Equals(ConnectionStatus.Status.Disconnected));
            }
        }
    }
}