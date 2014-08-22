using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class UserSnapshotRequestHandlerTest : MessageHandlerTestFixture
    {
        private readonly UserSnapshotRequestHandler userSnapshotRequestHandler = new UserSnapshotRequestHandler();

        private UserSnapshotRequest userSnapshotRequest;

        public override void HandleMessage(IMessage message)
        {
            userSnapshotRequestHandler.HandleMessage(message, ServiceRegistry);
        }

        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
            userSnapshotRequest = new UserSnapshotRequest(DefaultUser.Id);
        }

        [TestFixture]
        public class HandleMessageTest : UserSnapshotRequestHandlerTest
        {
            [Test]
            public void SendsAMessage()
            {
                bool isMessageSent = false;
                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => isMessageSent = true;

                HandleMessage(userSnapshotRequest);

                Assert.IsTrue(isMessageSent);
            }


            [Test]
            public void SendsAUserSnapshotMessage()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(userSnapshotRequest);

                Assert.IsTrue(message.MessageIdentifier == MessageIdentifier.UserSnapshot);
            }

            [Test]
            public void UserSnapshotSentContainsAllUsers()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(userSnapshotRequest);

                UserSnapshot userSnapshot = (UserSnapshot) message;

                IEnumerable<User> allUsers = ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>().GetAllEntities();

                Assert.AreEqual(userSnapshot.Users, allUsers);
            }

            [Test]
            public void ThrowsExceptionWhenMessageIsNotUserSnapshotRequest()
            {
                ConversationSnapshotRequest participationSnapshotRequest = new ConversationSnapshotRequest(DefaultUser.Id);
                Assert.Throws<InvalidCastException>(() => HandleMessage(participationSnapshotRequest));
            }
        }
    }
}