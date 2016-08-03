using System;
using System.Collections.Generic;
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
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
            userSnapshotRequest = new EntitySnapshotRequest<User>(DefaultUser.Id);
        }
        
        private EntitySnapshotRequest<User> userSnapshotRequest;

        public override void HandleMessage(IMessage message)
        {
            var userSnapshotRequestHandler = new UserSnapshotRequestHandler(ServiceRegistry);
            userSnapshotRequestHandler.HandleMessage(message);
        }

        [TestFixture]
        public class HandleMessageTest : UserSnapshotRequestHandlerTest
        {
            [Test]
            public void SendsAMessage()
            {
                var isMessageSent = false;
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
            public void ThrowsExceptionWhenMessageIsNotUserSnapshotRequest()
            {
                var participationSnapshotRequest = new EntitySnapshotRequest<Conversation>(DefaultUser.Id);
                Assert.Throws<InvalidCastException>(() => HandleMessage(participationSnapshotRequest));
            }

            [Test]
            public void UserSnapshotSentContainsAllUsers()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(userSnapshotRequest);

                var userSnapshot = (EntitySnapshot<User>) message;

                IEnumerable<User> allUsers = ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>().GetAllEntities();

                Assert.AreEqual(userSnapshot.Entities, allUsers);
            }
        }
    }
}