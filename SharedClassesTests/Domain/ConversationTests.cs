using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class ConversationTests
    {
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(12)]
        [TestCase(22)]
        public void AssignCustomIdToConversationTest(int conversationId)
        {
            var conversation = new Conversation(conversationId);
            Assert.AreEqual(conversation.ConversationId, conversationId);
        }

        [TestCase(12)]
        [TestCase(21326)]
        public void ConversationIdIterationTest(int conversationCount)
        {
            EntityGeneratorFactory entityGenerator = new EntityGeneratorFactory();
            int baseId = entityGenerator.GetEntityID<User>();

            int totalUsers = conversationCount;

            Conversation conversation = null;

            for (int i = 0; i < totalUsers; i++)
            {
                conversation = new Conversation(entityGenerator.GetEntityID<User>());
            }

            if (conversation != null)
            {
                Assert.AreEqual(conversation.ConversationId, totalUsers + baseId);
            }
        }

        [Test]
        public void AddContributionFromContributionRequestTest()
        {
            const string Message = "Hello";
            const int ConversationID = 5;

            var contribution = new Contribution(1, new Contribution(1, Message, ConversationID));
            var contributionRequest = new ContributionNotification(contribution);

            var conversation = new Conversation(ConversationID);

            conversation.AddContribution(contributionRequest);

            Assert.Contains(contribution, conversation.GetAllContributions().ToList());
        }

        [Test]
        public void AddContributionTest()
        {
            const int ConversationID = 5;

            var contribution = new Contribution(1, new Contribution(1, "Hello", ConversationID));
            var conversation = new Conversation(ConversationID);

            conversation.AddContribution(contribution);

            Assert.Contains(contribution, conversation.GetAllContributions().ToList());
        }

        [Test]
        public void ConversationEqualityTest()
        {
            var conversation = new Conversation(1);
            var otherConversation = new Conversation(1);
            Assert.AreEqual(conversation, otherConversation);
        }

        [Test]
        public void ConversationEqualsTest()
        {
            const int ConversationId = 4;
            var conversation = new Conversation(ConversationId);
            var conversation2 = new Conversation(ConversationId);
            Assert.AreEqual(conversation, conversation2);
            Assert.IsTrue(conversation.Equals(conversation2 as object));
        }

        [Test]
        public void ConversationHashCodeTest()
        {
            var conversation = new Conversation(1);
            Conversation conversation2 = conversation;

            Assert.AreEqual(conversation.GetHashCode(), conversation2.GetHashCode());
        }

        [Test]
        public void ConversationReferenceEqualsTest()
        {
            const int ConversationId = 4;

            var conversation = new Conversation(ConversationId);

            Conversation conversation2 = conversation;

            Assert.IsTrue(conversation.Equals(conversation2));
            Assert.IsTrue(conversation.Equals(conversation2 as object));
            Assert.IsFalse(conversation.Equals(null));

            object conversationObject = conversation;

            Assert.IsFalse(conversationObject.Equals(2));
            Assert.IsFalse(conversationObject.Equals(null));
        }

        [Test]
        public void GetAllContributionsTest()
        {
            const int ConversationID = 5;
            var contribution1 = new Contribution(1, new Contribution(1, "Hello", ConversationID));
            var contribution2 = new Contribution(2, new Contribution(1, "Hi...", ConversationID));
            var contribution3 = new Contribution(3, new Contribution(1, "Hello??", ConversationID));

            var conversation = new Conversation(ConversationID);

            conversation.AddContribution(contribution1);
            conversation.AddContribution(contribution2);
            conversation.AddContribution(contribution3);

            var contributionList = new List<Contribution>
            {
                contribution1,
                contribution2,
                contribution3
            };

            Assert.AreEqual(contributionList, conversation.GetAllContributions());
        }
    }
}