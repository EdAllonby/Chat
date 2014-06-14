using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class ParticipationTests
    {
        [Test]
        public void ParticipationEqualsTest()
        {
            const int ParticipationId = 1;
            const int UserId = 1;
            const int ConversationId = 2;

            var participation1 = new Participation(ParticipationId, UserId, ConversationId);
            var participation2 = new Participation(ParticipationId, UserId, ConversationId);

            Assert.AreEqual(participation1, participation2);
            Assert.IsTrue(participation1.Equals(participation2 as object));

            participation1 = new Participation(2, 1, 4);
            Assert.AreNotEqual(participation1, participation2);
        }

        [Test]
        public void ParticipationHashCodeTest()
        {
            const int ParticipationId = 1;
            const int UserId = 1;
            const int ConversationId = 2;

            var participation = new Participation(ParticipationId, UserId, ConversationId);
            var participation2 = new Participation(ParticipationId, UserId, ConversationId);
            Assert.AreEqual(participation.GetHashCode(), participation2.GetHashCode());
        }

        [Test]
        public void ParticipationReferenceEqualsTest()
        {
            const int ParticipationId = 1;
            const int UserId = 1;
            const int ConversationId = 2;

            var participation1 = new Participation(ParticipationId, UserId, ConversationId);
            Participation participation2 = participation1;

            Assert.IsTrue(participation1.Equals(participation2));
            Assert.IsTrue(participation1.Equals(participation2 as object));
            Assert.IsFalse(participation1.Equals(null));

            object participationObject = participation1;

            Assert.IsFalse(participationObject.Equals(2));
            Assert.IsFalse(participationObject.Equals(null));
        }

        [Test]
        public void ParticipationTest()
        {
            const int ParticipationId = 1;
            const int UserId = 1;
            const int ConversationId = 2;

            var participation = new Participation(ParticipationId, UserId, ConversationId);
            Assert.AreEqual(participation.UserId, UserId);
            Assert.AreEqual(participation.ConversationId, ConversationId);
        }
    }
}