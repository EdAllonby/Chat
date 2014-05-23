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
            ParticipationRepository participationRepository = new ParticipationRepository();

            Participation participation = new Participation(1, 1);
            participationRepository.AddParticipation(participation);
            Assert.AreEqual(participation, participationRepository.GetAllParticipations().First());
        }

        [Test]
        public void AddParticipationsTest()
        {
            ParticipationRepository participationRepository = new ParticipationRepository();

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
            ParticipationRepository participationRepository = new ParticipationRepository();
            Participation participation = new Participation(1, 1);
            participationRepository.AddParticipation(participation);

            Assert.True(participationRepository.DoesConversationWithUsersExist(new List<int> {participation.UserId}));
        }

        [Test]
        public void GetParticipationsByConversationIdTest()
        {
            ParticipationRepository participationRepository = new ParticipationRepository();
            int conversationId = 10;
            Participation participation1 = new Participation(1, conversationId);
            Participation participation2 = new Participation(2, conversationId);

            participationRepository.AddParticipation(participation1);
            participationRepository.AddParticipation(participation2);

            List<Participation> expectedParticipations = new List<Participation> {participation1, participation2};

            IEnumerable<Participation> actualParticipations =
                participationRepository.GetParticipationsByConversationId(conversationId);

            Assert.AreEqual(expectedParticipations, actualParticipations);
        }

        [Test]
        public void GetConversationIdByParticipantsIdTest()
        {
            ParticipationRepository participationRepository = new ParticipationRepository();
            int conversationId = 10;
            Participation participation1 = new Participation(1, conversationId);
            Participation participation2 = new Participation(2, conversationId);

            participationRepository.AddParticipation(participation1);
            participationRepository.AddParticipation(participation2);

            List<int> participantIds = new List<int> {participation1.UserId, participation2.UserId};

            int actualConversationId = participationRepository.GetConversationIdByParticipantsId(participantIds);

            Assert.AreEqual(conversationId, actualConversationId);
        }

        [Test]
        public void GetAllConversationIdsByUserIdTest()
        {
            ParticipationRepository participationRepository = new ParticipationRepository();

            int userId = 3;

            Participation participation1 = new Participation(userId, 1);
            Participation participation2 = new Participation(userId, 2);
            Participation participation3 = new Participation(userId, 3);

            List<int> expectedConversationIds = new List<int>
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
            ParticipationRepository participationRepository = new ParticipationRepository();

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
    }
}