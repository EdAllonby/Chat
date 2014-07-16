using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class ConversationRepositoryTests
    {
        [Test]
        public void AddConversationEntitiesTest()
        {
            var conversationRepository = new ConversationRepository();

            var conversations = new List<Conversation> {new Conversation(1), new Conversation(2)};

            conversationRepository.AddConversations(conversations);
            Assert.AreEqual(conversations, conversationRepository.GetAllConversations());
        }

        [Test]
        public void AddConversationEntityTest()
        {
            var conversation = new Conversation(1);
            var conversationRepository = new ConversationRepository();
            conversationRepository.AddConversation(conversation);
            Assert.AreEqual(conversation, conversationRepository.FindConversationById(conversation.ConversationId));
        }

        [Test]
        public void CanNotAddSameConversationEntityTwice()
        {
            var conversation = new Conversation(1);

            var conversationRepository = new ConversationRepository();

            conversationRepository.AddConversation(conversation);
            Assert.Throws<ArgumentException>(() => conversationRepository.AddConversation(conversation));
        }

        [Test]
        public void FindNonExistentConversationTest()
        {
            var conversationRepository = new ConversationRepository();

            Assert.IsNull(conversationRepository.FindConversationById(3));
        }
    }
}