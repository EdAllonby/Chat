using System;
using NUnit.Framework;
using Server.MessageHandler;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ServerTests.MessageHandlerTests
{
    [TestFixture]
    public class ContributionRequestHandlerTest : MessageHandlerTestFixture
    {
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            contributionRequest = new ContributionRequest(ConversationId, ConnectedUserId, "hello");
            ConversationRepository.AddEntity(new Conversation(ConversationId));
        }

        private ContributionRequest contributionRequest;
        private readonly ContributionRequestHandler contributionRequestHandler = new ContributionRequestHandler();
        private const int ConversationId = 1;

        private IRepository<Conversation> ConversationRepository
        {
            get { return (IRepository<Conversation>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>(); }
        }

        [TestFixture]
        public class HandleMessage : ContributionRequestHandlerTest
        {
            [Test]
            public void AddsNewContributionToConversation()
            {
                contributionRequestHandler.HandleMessage(contributionRequest, ServiceRegistry);
                Contribution lastContributionAdded = ConversationRepository.FindEntityById(contributionRequest.Contribution.ConversationId).LastContribution;

                Assert.IsTrue(lastContributionAdded.Message.Equals(contributionRequest.Contribution.Message));
            }

            [Test]
            public void NewContributionGetsAssignedId()
            {
                contributionRequestHandler.HandleMessage(contributionRequest, ServiceRegistry);
                Contribution lastContributionAdded = ConversationRepository.FindEntityById(contributionRequest.Contribution.ConversationId).LastContribution;

                Assert.IsTrue(lastContributionAdded.Id != 0);
            }

            [Test]
            public void ThrowsExceptionWhenNotGivenContributionRequest()
            {
                Assert.Throws<InvalidCastException>(() => contributionRequestHandler.HandleMessage(new ClientDisconnection(9), ServiceRegistry));
            }
        }
    }
}