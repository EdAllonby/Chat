using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class ParticipationSnapshotRequestHandlerTest : MessageHandlerTestFixture
    {
        private readonly ParticipationSnapshotRequestHandler participationSnapshotRequestHandler = new ParticipationSnapshotRequestHandler();

        private ParticipationSnapshotRequest participationSnapshotRequest;

        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            participationSnapshotRequest = new ParticipationSnapshotRequest(DefaultUser.Id);
        }

        public override void HandleMessage(IMessage message)
        {
            participationSnapshotRequestHandler.HandleMessage(message, ServiceRegistry);
        }

        [TestFixture]
        public class HandleMessageTest : ParticipationSnapshotRequestHandlerTest
        {
            [Test]
            public void SendsAMessage()
            {
                bool isMessageSent = false;
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
            public void ParticipationSnapshotSentContainsAllConversationsUserIsIn()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(participationSnapshotRequest);

                ParticipationSnapshot conversationSnapshot = (ParticipationSnapshot) message;

                List<int> conversationIds = conversationSnapshot.Participations.Select(participation => participation.ConversationId).ToList();

                Assert.AreEqual(DefaultConversationDefaultUserIsIn, conversationIds.Distinct().First());
            }
            
            [Test]
            public void ParticipationSnapshotIsForCorrectUser()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(participationSnapshotRequest);

                ParticipationSnapshot conversationSnapshot = (ParticipationSnapshot)message;

                int userId = conversationSnapshot.Participations.Select(participation => participation.UserId).First();

                Assert.AreEqual(DefaultUser.Id, userId);
            }

            [Test]
            public void ThrowsExceptionWhenGivenMessageThatIsNotParticipationSnapshot()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new UserSnapshotRequest(31)));
            }
        }
    }
}