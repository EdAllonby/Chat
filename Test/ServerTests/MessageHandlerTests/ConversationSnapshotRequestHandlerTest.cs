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
    public class ConversationSnapshotRequestHandlerTest : MessageHandlerTestFixture
    {
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            conversationSnapshotRequest = new EntitySnapshotRequest<Conversation>(DefaultUser.Id);
        }

        private EntitySnapshotRequest<Conversation> conversationSnapshotRequest;

        public override void HandleMessage(IMessage message)
        {
            var conversationSnapshotRequestHandler = new ConversationSnapshotRequestHandler(ServiceRegistry);
            conversationSnapshotRequestHandler.HandleMessage(message);
        }

        [TestFixture]
        public class HandleMessageTest : ConversationSnapshotRequestHandlerTest
        {
            [Test]
            public void ConversationSnapshotSentContainsAllConversationsUserIsIn()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(conversationSnapshotRequest);

                var conversationSnapshot = (EntitySnapshot<Conversation>) message;

                List<int> conversationIds = conversationSnapshot.Entities.Select(conversation => conversation.Id).ToList();

                Assert.AreEqual(DefaultConversationIdDefaultUserIsIn, conversationIds.Distinct().First());
            }

            [Test]
            public void SendsAConversationSnapshotMessage()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(conversationSnapshotRequest);

                Assert.IsTrue(message.MessageIdentifier == MessageIdentifier.ConversationSnapshot);
            }

            [Test]
            public void SendsAMessage()
            {
                var isMessageSent = false;
                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => isMessageSent = true;

                HandleMessage(conversationSnapshotRequest);

                Assert.IsTrue(isMessageSent);
            }

            [Test]
            public void ThrowsExceptionWhenMessageIsNotConversationSnapshotRequest()
            {
                var participationSnapshotRequest = new EntitySnapshotRequest<Participation>(DefaultUser.Id);
                Assert.Throws<InvalidCastException>(() => HandleMessage(participationSnapshotRequest));
            }
        }
    }
}