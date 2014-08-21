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

            UserRepository.AddEntity(userToUpdate);
        }

        private const int UserId = 1;

        private readonly AvatarRequestHandler avatarRequestHandler = new AvatarRequestHandler();
        private AvatarRequest avatarRequest;
        private User userToUpdate;

        private IRepository<User> UserRepository
        {
            get { return (IRepository<User>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>(); }
        }

        [TestFixture]
        public class HandleMessageTest : AvatarRequestHandlerTest
        {
            [Test]
            public void AvatarGetsAssignedId()
            {
                avatarRequestHandler.HandleMessage(avatarRequest, ServiceRegistry);

                Assert.IsTrue(UserRepository.FindEntityById(UserId).Avatar.Id != 0);
            }

            [Test]
            public void ThrowsExceptionWhenNonAvatarRequestIsPassed()
            {
                Assert.Throws<InvalidCastException>(() => avatarRequestHandler.HandleMessage(new LoginRequest("login"), ServiceRegistry));
            }

            [Test]
            public void UserGetsUpdatedAvatarInUserRepository()
            {
                avatarRequestHandler.HandleMessage(avatarRequest, ServiceRegistry);

                Assert.IsTrue(UserRepository.FindEntityById(UserId).Avatar.UserAvatar.Size.Equals(Resources.SmallImage.Size));
            }

            [Test]
            public void UserRepositoryUpdatesUser()
            {
                bool isUserUpdated = false;
                UserRepository.EntityUpdated += (sender, eventArgs) => isUserUpdated = true;

                avatarRequestHandler.HandleMessage(avatarRequest, ServiceRegistry);

                Assert.IsTrue(isUserUpdated);
            }
        }
    }
}