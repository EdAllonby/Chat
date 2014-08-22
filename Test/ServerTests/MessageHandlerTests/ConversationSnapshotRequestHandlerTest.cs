﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class ConversationSnapshotRequestHandlerTest : MessageHandlerTestFixture
    {
        private readonly ConversationSnapshotRequestHandler conversationSnapshotRequestHandler =
            new ConversationSnapshotRequestHandler();

        private ConversationSnapshotRequest conversationSnapshotRequest;

        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            conversationSnapshotRequest = new ConversationSnapshotRequest(DefaultUser.Id);
        }

        public override void HandleMessage(IMessage message)
        {
            conversationSnapshotRequestHandler.HandleMessage(message, ServiceRegistry);
        }

        [TestFixture]
        public class HandleMessageTest : ConversationSnapshotRequestHandlerTest
        {
            [Test]
            public void SendsAMessage()
            {
                bool isMessageSent = false;
                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => isMessageSent = true;

                HandleMessage(conversationSnapshotRequest);

                Assert.IsTrue(isMessageSent);
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
            public void ConversationSnapshotSentContainsAllConversationsUserIsIn()
            {
                IMessage message = null;

                ConnectedUserClientHandler.MessageSent += (sender, eventArgs) => message = eventArgs.Message;

                HandleMessage(conversationSnapshotRequest);

                ConversationSnapshot conversationSnapshot = (ConversationSnapshot) message;

                List<int> conversationIds = conversationSnapshot.Conversations.Select(conversation => conversation.Id).ToList();

                Assert.AreEqual(DefaultConversationDefaultUserIsIn, conversationIds.Distinct().First());
            }

            [Test]
            public void ThrowsExceptionWhenMessageIsNotConversationSnapshotRequest()
            {
                ParticipationSnapshotRequest participationSnapshotRequest = new ParticipationSnapshotRequest(DefaultUser.Id);
                Assert.Throws<InvalidCastException>(() => HandleMessage(participationSnapshotRequest));
            }
        }
    }
}