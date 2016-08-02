using System;
using NUnit.Framework;
using Server.MessageHandler;
using ServerTests.Properties;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class AvatarRequestHandlerTest : MessageHandlerTestFixture
    {
        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            avatarRequest = new AvatarRequest(UserId, Resources.SmallImage);
            userToUpdate = new User("user", UserId, new ConnectionStatus(UserId, ConnectionStatus.Status.Connected));
        }

        public override void HandleMessage(IMessage message)
        {
            avatarRequestHandler.HandleMessage(message, ServiceRegistry);
        }

        private const int UserId = 1;

        private readonly AvatarRequestHandler avatarRequestHandler = new AvatarRequestHandler();
        private AvatarRequest avatarRequest;
        private User userToUpdate;

        private IEntityRepository<User> UserRepository => (IEntityRepository<User>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

        [TestFixture]
        public class HandleMessageTest : AvatarRequestHandlerTest
        {
            [Test]
            public void AvatarGetsAssignedId()
            {
                HandleMessage(avatarRequest);

                Assert.IsTrue(UserRepository.FindEntityById(UserId).Avatar.Id != 0);
            }

            [Test]
            public void ThrowsExceptionWhenNonAvatarRequestIsPassed()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new LoginRequest("login")));
            }

            [Test]
            public void UserGetsUpdatedAvatarInUserRepository()
            {
                HandleMessage(avatarRequest);

                Assert.IsTrue(UserRepository.FindEntityById(UserId).Avatar.UserAvatar.Size.Equals(Resources.SmallImage.Size));
            }

            [Test]
            public void UserRepositoryUpdatesUser()
            {
                var isUserUpdated = false;
                UserRepository.EntityUpdated += (sender, eventArgs) => isUserUpdated = true;

                HandleMessage(avatarRequest);

                Assert.IsTrue(isUserUpdated);
            }
        }
    }
}