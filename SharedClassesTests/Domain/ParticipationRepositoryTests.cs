using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class ParticipationRepositoryTests
    {
        [Test]
        public void AddParticipationTest()
        {
            var participationRepository = new ParticipationRepository();

            var participation = new Participation(1, 1);
            participationRepository.AddParticipation(participation);
            Assert.AreEqual(participation, participationRepository.GetAllParticipations().First());
        }

        [Test]
        public void AddParticipationsTest()
        {
            var participationRepository = new ParticipationRepository();

            int conversationId = 1;
            IList<Participation> participations = new List<Participation>
            {
                new Participation(1, conversationId),
                new Participation(2, conversationId)
            };
            participationRepository.AddParticipations(participations);

            Assert.AreEqual(participations, participationRepository.GetAllParticipations());
        }

        [Test]
        public void DoesConversationWithUsersExistTest()
        {
            var participationRepository = new ParticipationRepository();
            var participation = new Participation(1, 1);
            participationRepository.AddParticipation(participation);

            Assert.True(participationRepository.DoesConversationWithUsersExist(new List<int> {participation.UserId}));
        }

        [Test]
        public void GetAllConversationIdsByUserIdTest()
        {
            var participationRepository = new ParticipationRepository();

            int userId = 3;

            var participation1 = new Participation(userId, 1);
            var participation2 = new Participation(userId, 2);
            var participation3 = new Participation(userId, 3);

            var expectedConversationIds = new List<int>
            {
                participation1.ConversationId,
                participation2.ConversationId,
                participation3.ConversationId
            };

            participationRepository.AddParticipation(participation1);
            participationRepository.AddParticipation(participation2);
            participationRepository.AddParticipation(participation3);

            IEnumerable<int> actualConversationIds = participationRepository.GetAllConversationIdsByUserId(userId);

            Assert.AreEqual(expectedConversationIds, actualConversationIds);
        }

        [Test]
        public void GetAllParticipationsTest()
        {
            var participationRepository = new ParticipationRepository();

            int conversationId = 1;
            IList<Participation> participations = new List<Participation>
            {
                new Participation(1, conversationId),
                new Participation(2, conversationId),
                new Participation(3, conversationId),
                new Participation(5, conversationId)
            };

            participationRepository.AddParticipations(participations);

            Assert.AreEqual(participations, participationRepository.GetAllParticipations());
        }

        [Test]
        public void GetConversationIdByParticipantsIdTest()
        {
            var participationRepository = new ParticipationRepository();
            int conversationId = 10;
            var participation1 = new Participation(1, conversationId);
            var participation2 = new Participation(2, conversationId);

            participationRepository.AddParticipation(participation1);
            participationRepository.AddParticipation(participation2);

            var participantIds = new List<int> {participation1.UserId, participation2.UserId};

            int actualConversationId = participationRepository.GetConversationIdByParticipantsId(participantIds);

            Assert.AreEqual(conversationId, actualConversationId);
        }

        [Test]
        public void GetParticipationsByConversationIdTest()
        {
            var participationRepository = new ParticipationRepository();
            int conversationId = 10;
            var participation1 = new Participation(1, conversationId);
            var participation2 = new Participation(2, conversationId);

            participationRepository.AddParticipation(participation1);
            participationRepository.AddParticipation(participation2);

            var expectedParticipations = new List<Participation> {participation1, participation2};

            IEnumerable<Participation> actualParticipations =
                participationRepository.GetParticipationsByConversationId(conversationId);

            Assert.AreEqual(expectedParticipations, actualParticipations);
        }
    }
}