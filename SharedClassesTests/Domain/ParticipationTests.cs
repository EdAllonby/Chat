using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class ParticipationTests
    {
        [Test]
        public void ParticipationTest()
        {
            const int UserId = 1;
            const int ConversationId = 2;
            Participation participation = new Participation(UserId, ConversationId);
            Assert.AreEqual(participation.UserId, UserId);
            Assert.AreEqual(participation.ConversationId, ConversationId);
        }

        [Test]
        public void EqualsTest()
        {
            const int UserId = 1;
            const int ConversationId = 2;
            Participation participation1 = new Participation(UserId, ConversationId);
            Participation participation2 = new Participation(UserId, ConversationId);
            Assert.AreEqual(participation1, participation2);
        }
    }
}