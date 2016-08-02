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
        public void AddParticipationsTest()
        {
            var participationRepository = new ParticipationRepository();

            var conversationId = 1;
            IList<Participation> participations = new List<Participation>
            {
                new Participation(1, 1, conversationId),
                new Participation(2, 2, conversationId)
            };

            foreach (Participation participation in participations)
            {
                participationRepository.AddEntity(participation);
            }

            Assert.AreEqual(participations, participationRepository.GetAllEntities());
        }

        [Test]
        public void AddParticipationTest()
        {
            var participationRepository = new ParticipationRepository();

            var participation = new Participation(1, 1, 1);
            participationRepository.AddEntity(participation);
            Assert.AreEqual(participation, participationRepository.GetAllEntities().First());
        }

        [Test]
        public void DoesConversationWithUsersExistTest()
        {
            var participationRepository = new ParticipationRepository();
            var participation = new Participation(1, 1, 1);
            participationRepository.AddEntity(participation);

            Assert.True(participationRepository.DoesConversationWithUsersExist(new List<int> { participation.UserId }));
        }

        [Test]
        public void GetAllConversationIdsByUserIdTest()
        {
            var participationRepository = new ParticipationRepository();

            var userId = 3;

            var participation1 = new Participation(1, userId, 1);
            var participation2 = new Participation(2, userId, 2);
            var participation3 = new Participation(3, userId, 3);

            var expectedConversationIds = new List<int>
            {
                participation1.ConversationId,
                participation2.ConversationId,
                participation3.ConversationId
            };

            participationRepository.AddEntity(participation1);
            participationRepository.AddEntity(participation2);
            participationRepository.AddEntity(participation3);

            IEnumerable<int> actualConversationIds = participationRepository.GetAllConversationIdsByUserId(userId);

            Assert.AreEqual(expectedConversationIds, actualConversationIds);
        }

        [Test]
        public void GetAllParticipationsTest()
        {
            var participationRepository = new ParticipationRepository();

            var conversationId = 1;
            IList<Participation> participations = new List<Participation>
            {
                new Participation(1, 1, conversationId),
                new Participation(2, 2, conversationId),
                new Participation(3, 3, conversationId),
                new Participation(4, 5, conversationId)
            };
            foreach (Participation participation in participations)
            {
                participationRepository.AddEntity(participation);
            }

            Assert.AreEqual(participations, participationRepository.GetAllEntities());
        }

        [Test]
        public void GetConversationIdByParticipantsIdTest()
        {
            var participationRepository = new ParticipationRepository();
            var conversationId = 10;
            var participation1 = new Participation(1, 1, conversationId);
            var participation2 = new Participation(2, 2, conversationId);

            participationRepository.AddEntity(participation1);
            participationRepository.AddEntity(participation2);

            var participantIds = new List<int> { participation1.UserId, participation2.UserId };

            int actualConversationId = participationRepository.GetConversationIdByUserIds(participantIds);

            Assert.AreEqual(conversationId, actualConversationId);
            Assert.AreEqual(0, participationRepository.GetConversationIdByUserIds(new List<int> { 1, 2, 3 }));
        }

        [Test]
        public void GetParticipationsByConversationIdTest()
        {
            const int conversationId = 10;

            var participationRepository = new ParticipationRepository();
            var participation1 = new Participation(1, 1, conversationId);
            var participation2 = new Participation(2, 2, conversationId);

            participationRepository.AddEntity(participation1);
            participationRepository.AddEntity(participation2);

            var expectedParticipations = new List<Participation> { participation1, participation2 };

            IEnumerable<Participation> actualParticipations =
                participationRepository.GetParticipationsByConversationId(conversationId);

            Assert.AreEqual(expectedParticipations, actualParticipations);
        }
    }
}