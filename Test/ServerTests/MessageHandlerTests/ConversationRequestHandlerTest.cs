using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class ConversationRequestHandlerTest : MessageHandlerTestFixture
    {
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
            usersInConversation = new List<int> { DefaultUser.Id, 3 };
            conversationRequest = new ConversationRequest(usersInConversation);
        }


        private List<int> usersInConversation;

        private IReadOnlyEntityRepository<Conversation> ConversationRepository => ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

        private static Conversation GetNewlyAddedConversation(IEnumerable<Conversation> newConversationList, IEnumerable<Conversation> previousConversationList)
        {
            return newConversationList.Except(previousConversationList).First();
        }

        private ConversationRequest conversationRequest;

        public override void HandleMessage(IMessage message)
        {
            var conversationRequestHandler = new ConversationRequestHandler(ServiceRegistry);
            conversationRequestHandler.HandleMessage(message);
        }

        private Conversation HandleMessageAndGetAddedConversation(IMessage message)
        {
            IEnumerable<Conversation> previousConversationInRepository = ConversationRepository.GetAllEntities();
            HandleMessage(message);
            IEnumerable<Conversation> newConversationsInRepository = ConversationRepository.GetAllEntities();

            return GetNewlyAddedConversation(newConversationsInRepository, previousConversationInRepository);
        }

        [TestFixture]
        public class HandleMessageTest : ConversationRequestHandlerTest
        {
            [Test]
            public void AddsNewConversationToRepository()
            {
                Conversation newConversation = HandleMessageAndGetAddedConversation(conversationRequest);

                Assert.IsNotNull(newConversation);
            }

            [Test]
            public void AssignsNewConversationAnId()
            {
                Conversation newConversation = HandleMessageAndGetAddedConversation(conversationRequest);

                Assert.IsTrue(newConversation.Id != 0);
            }

            [Test]
            public void DoesNotProcessConversationRequestIfThereAreDuplicateConversationUserIdsInRequest()
            {
                var usersToMakeInConversation = new List<int> { 1, 2, 3, 4, 1 };
                var invalidConversationRequest = new ConversationRequest(usersToMakeInConversation);
                IEnumerable<Conversation> currentConversationsInRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>().GetAllEntities();
                IEnumerable<Participation> currentParticipationsInRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>().GetAllEntities();
                var previousParticipationsInRepository = new List<Participation>(currentParticipationsInRepository);
                var previousConversationsInRepository = new List<Conversation>(currentConversationsInRepository);

                HandleMessage(invalidConversationRequest);

                Assert.AreEqual(previousConversationsInRepository, currentConversationsInRepository);
                Assert.AreEqual(previousParticipationsInRepository, currentParticipationsInRepository);
            }

            [Test]
            public void ParticipationRepositoryContainsNewConversationToUserMap()
            {
                Conversation newConversation = HandleMessageAndGetAddedConversation(conversationRequest);

                var participationRepository = (ParticipationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
                List<Participation> newConversationParticipants = participationRepository.GetParticipationsByConversationId(newConversation.Id);

                List<int> participantUserIds = newConversationParticipants.Select(item => item.UserId).ToList();

                Assert.AreEqual(participantUserIds, usersInConversation);
            }

            [Test]
            public void ThrowExceptionWhenNotGivenConversationRequest()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new EntitySnapshotRequest<Participation>(10)));
            }
        }
    }
}