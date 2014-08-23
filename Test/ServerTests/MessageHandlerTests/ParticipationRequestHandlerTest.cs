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
        private readonly ParticipationRequestHandler participationRequestHandler = new ParticipationRequestHandler();
        private ParticipationRepository participationRepository;
        private User newParticipant;

        public override void HandleMessage(IMessage message)
        {
            participationRequestHandler.HandleMessage(message, ServiceRegistry);
        }

        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
            participationRepository = (ParticipationRepository)ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            int newUserId = ServiceRegistry.GetService<EntityIdAllocatorFactory>().AllocateEntityId<User>();
            newParticipant = new User("new User", newUserId, new ConnectionStatus(newUserId, ConnectionStatus.Status.Connected));
            UserRepository userRepository = (UserRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();
            userRepository.AddEntity(newParticipant);
        }

        [TestFixture]
        public class HandleMessageTest : ParticipationRequestHandlerTest
        {
            [Test]
            public void DoNothingIfNewParticipantIsAlreadyInConversation()
            {
                List<Participation> previousParticipationsLinkedWithConversation = new List<Participation>(participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn));
                Participation duplicateParticipation = previousParticipationsLinkedWithConversation.First();
                ParticipationRequest participationRequest = new ParticipationRequest(duplicateParticipation);
                HandleMessage(participationRequest);

                List<Participation> newParticipationsLinkedWithConversation = participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn);

                Assert.That(previousParticipationsLinkedWithConversation, Is.EquivalentTo(newParticipationsLinkedWithConversation));
            }

            [Test]
            public void AddsParticipationToRepositoryIfParticipantIsValid()
            {
                ParticipationRequest participationRequest = new ParticipationRequest(new Participation(newParticipant.Id, DefaultConversationIdDefaultUserIsIn));
                HandleMessage(participationRequest);
                var newParticipationsLinkedWithConversation = participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn);
                Assert.IsTrue(newParticipationsLinkedWithConversation.Contains(participationRequest.Participation));
            }

            [Test]
            public void NewParticipationGetsAssignedId()
            {
                ParticipationRequest participationRequest = new ParticipationRequest(new Participation(newParticipant.Id, DefaultConversationIdDefaultUserIsIn));
                HandleMessage(participationRequest);
                var newParticipationsLinkedWithConversation = participationRepository.GetParticipationsByConversationId(DefaultConversationIdDefaultUserIsIn);
                Assert.IsTrue(newParticipationsLinkedWithConversation.All(participation => participation.Id > 0));
            }

            [Test]
            public void ThrowsExceptionWhenNotPassedParticipationRequest()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new ParticipationSnapshot(new List<Participation>())));
            }
        }
    }
}