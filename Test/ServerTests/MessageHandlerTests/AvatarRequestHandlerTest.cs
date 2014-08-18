using System;
using NUnit.Framework;
using Server;
using Server.MessageHandler;
using ServerTests.Properties;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class AvatarRequestHandlerTest
    {
        private const int UserId = 1;

        private AvatarRequest avatarRequest;
        private User userToUpdate;
        private RepositoryManager repositoryManager;
        private IServerMessageContext serverMessageContext;
        private AvatarRequestHandler avatarRequestHandler;

        private UserRepository UserRepository
        {
            get { return repositoryManager.UserRepository; }
        }

        [SetUp]
        public void BeforeEachTest()
        {
            avatarRequest = new AvatarRequest(UserId, Resources.SmallImage);
            userToUpdate = new User("user", UserId, new ConnectionStatus(UserId, ConnectionStatus.Status.Connected));
            repositoryManager = new RepositoryManager();
            repositoryManager.UserRepository.AddEntity(userToUpdate);
            serverMessageContext = new ServerMessageContext(new ClientManager(), new EntityIdAllocatorFactory(), repositoryManager);
            avatarRequestHandler = new AvatarRequestHandler();
        }

        [TestFixture]
        public class HandleMessage : AvatarRequestHandlerTest
        {
            [Test]
            public void UserGetsUpdatedAvatarInUserRepository()
            {
                avatarRequestHandler.HandleMessage(avatarRequest, serverMessageContext);

                Assert.IsTrue(UserRepository.FindEntityById(UserId).Avatar.UserAvatar.Size.Equals(Resources.SmallImage.Size));
            }

            [Test]
            public void AvatarGetsAssignedId()
            {
                avatarRequestHandler.HandleMessage(avatarRequest, serverMessageContext);

                Assert.IsTrue(UserRepository.FindEntityById(UserId).Avatar.Id != 0);
            }

            [Test]
            public void UserRepositoryUpdatesUser()
            {
                bool isUserUpdated = false;
                UserRepository.EntityUpdated += (sender, eventArgs) => isUserUpdated = true;

                avatarRequestHandler.HandleMessage(avatarRequest, serverMessageContext);

                Assert.IsTrue(isUserUpdated);
            }

            [Test]
            public void ThrowsExceptionWhenNonAvatarRequestIsPassed()
            {
                Assert.Throws<InvalidCastException>(() => avatarRequestHandler.HandleMessage(new LoginRequest("login"), serverMessageContext));
            }
        }
    }
}