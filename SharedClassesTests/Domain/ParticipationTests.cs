using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class ParticipationTests
    {
        [Test]
        public void EqualsTest()
        {
            const int UserId = 1;
            const int ConversationId = 2;
            var participation1 = new Participation(UserId, ConversationId);
            var participation2 = new Participation(UserId, ConversationId);
            Assert.AreEqual(participation1, participation2);
        }

        [Test]
        public void ParticipationTest()
        {
            const int UserId = 1;
            const int ConversationId = 2;
            var participation = new Participation(UserId, ConversationId);
            Assert.AreEqual(participation.UserId, UserId);
            Assert.AreEqual(participation.ConversationId, ConversationId);
        }
    }
}