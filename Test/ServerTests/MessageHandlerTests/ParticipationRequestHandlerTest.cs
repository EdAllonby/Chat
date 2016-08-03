using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Server;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class ParticipationRequestHandlerTest : MessageHandlerTestFixture
    {
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
            participationRepository = (ParticipationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            int newUserId = ServiceRegistry.GetService<EntityIdAllocatorFactory>().AllocateEntityId<User>();
            newParticipant = new User("new User", newUserId, new ConnectionStatus(newUserId, ConnectionStatus.Status.Connected));
            var userRepository = (UserRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();
            userRepository.AddEntity(newParticipant);
        }

        private ParticipationRepository participationRepository;
        private User newParticipant;

        public override void HandleMessage(IMessage message)
        {
            var participationRequestHandler = new ParticipationRequestHandler(ServiceRegistry);
            participationRequestHandler.HandleMessage(message);
        }

        [TestFixture]
        public class HandleMessageTest : ParticipationRequestHandlerTest
        {
            [Test]
            public void AddsParticipationToRepositoryIfParticipantIsValid()
            {
                var participationRequest = new ParticipationRequest(new Participation(newParticipant.Id, DefaultConversationIdDefaultUserIsIn));
                HandleMessage(participationRequest);
                List<Participation> newParticipationsLinkedWithConversation = participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn);
                Assert.IsTrue(newParticipationsLinkedWithConversation.Contains(participationRequest.Participation));
            }

            [Test]
            public void DoNothingIfNewParticipantIsAlreadyInConversation()
            {
                var previousParticipationsLinkedWithConversation = new List<Participation>(participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn));
                Participation duplicateParticipation = previousParticipationsLinkedWithConversation.First();
                var participationRequest = new ParticipationRequest(duplicateParticipation);
                HandleMessage(participationRequest);

                List<Participation> newParticipationsLinkedWithConversation = participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn);

                Assert.That(previousParticipationsLinkedWithConversation, Is.EquivalentTo(newParticipationsLinkedWithConversation));
            }

            [Test]
            public void NewParticipationGetsAssignedId()
            {
                var participationRequest = new ParticipationRequest(new Participation(newParticipant.Id, DefaultConversationIdDefaultUserIsIn));
                HandleMessage(participationRequest);
                List<Participation> newParticipationsLinkedWithConversation = participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn);
                Assert.IsTrue(newParticipationsLinkedWithConversation.All(participation => participation.Id > 0));
            }

            [Test]
            public void ThrowsExceptionWhenNotPassedParticipationRequest()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new EntitySnapshot<Participation>(new List<Participation>())));
            }
        }
    }
}