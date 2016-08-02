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

            var conversations = new List<Conversation> { new Conversation(1), new Conversation(2) };

            foreach (Conversation conversation in conversations)
            {
                conversationRepository.AddEntity(conversation);
            }

            Assert.AreEqual(conversations, conversationRepository.GetAllEntities());
        }

        [Test]
        public void AddConversationEntityTest()
        {
            var conversation = new Conversation(1);
            var conversationRepository = new ConversationRepository();
            conversationRepository.AddEntity(conversation);
            Assert.AreEqual(conversation, conversationRepository.FindEntityById(conversation.Id));
        }
    }
}