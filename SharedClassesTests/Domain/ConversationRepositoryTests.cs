using System.Collections.Generic;
using System.Linq;
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
            var conversation1 = new Conversation(1);
            var conversation2 = new Conversation(2);
            var conversations = new List<Conversation> {conversation1, conversation2};

            var conversationRepository = new ConversationRepository();

            conversationRepository.AddConversations(conversations);
            Assert.AreEqual(conversations, conversationRepository.GetAllEntities().ToList());
        }

        [Test]
        public void AddConversationEntityTest()
        {
            var conversation = new Conversation(1);
            var conversationRepository = new ConversationRepository();
            conversationRepository.AddEntity(conversation);
            Assert.AreEqual(conversation, conversationRepository.FindEntityByID(conversation.ConversationId));
        }

        [Test]
        public void CanNotAddSameConversationEntityTwice()
        {
            var conversation = new Conversation(1);

            var conversationRepository = new ConversationRepository();

            conversationRepository.AddEntity(conversation);
            conversationRepository.AddEntity(conversation);

            var conversations = new List<Conversation> {conversation};

            Assert.AreEqual(conversations, conversationRepository.GetAllEntities());
        }
    }
}