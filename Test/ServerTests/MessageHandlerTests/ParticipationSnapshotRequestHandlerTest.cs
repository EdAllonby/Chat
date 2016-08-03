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
    public class ParticipationSnapshotRequestHandlerTest : MessageHandlerTestFixture
    {
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            participationSnapshotRequest = new EntitySnapshotRequest<Participation>(DefaultUser.Id);
        }

        private EntitySnapshotRequest<Participation> participationSnapshotRequest;

        public override void HandleMessage(IMessage message)
        {
            var participationSnapshotRequestHandler = new ParticipationSnapshotRequestHandler(ServiceRegistry);
            participationSnapshotRequestHandler.HandleMessage(message);
        }

        [TestFixture]
        public class HandleMessageTest : ParticipationSnapshotRequestHandlerTest
        {
            [Test]
            public void ParticipationSnapshotIsForCorrectUser()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(participationSnapshotRequest);

                var conversationSnapshot = (EntitySnapshot<Participation>) message;

                int userId = conversationSnapshot.Entities.Select(participation => participation.UserId).First();

                Assert.AreEqual(DefaultUser.Id, userId);
            }

            [Test]
            public void ParticipationSnapshotSentContainsAllConversationsUserIsIn()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(participationSnapshotRequest);

                var conversationSnapshot = (EntitySnapshot<Participation>) message;

                List<int> conversationIds = conversationSnapshot.Entities.Select(participation => participation.ConversationId).ToList();

                Assert.AreEqual(DefaultConversationIdDefaultUserIsIn, conversationIds.Distinct().First());
            }

            [Test]
            public void SendsAMessage()
            {
                var isMessageSent = false;
                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => isMessageSent = true;

                HandleMessage(participationSnapshotRequest);

                Assert.IsTrue(isMessageSent);
            }

            [Test]
            public void SendsAParticipationSnapshotMessage()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(participationSnapshotRequest);

                Assert.IsTrue(message.MessageIdentifier == MessageIdentifier.ParticipationSnapshot);
            }

            [Test]
            public void ThrowsExceptionWhenGivenMessageThatIsNotParticipationSnapshot()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new EntitySnapshotRequest<User>(31)));
            }
        }
    }
}