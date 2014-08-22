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

            contributionRequest = new ContributionRequest(ConversationId, DefaultUser.Id, "hello");
            ConversationRepository.AddEntity(new Conversation(ConversationId));
        }

        public override void HandleMessage(IMessage message)
        {
            contributionRequestHandler.HandleMessage(message, ServiceRegistry);
        }

        private ContributionRequest contributionRequest;
        private readonly ContributionRequestHandler contributionRequestHandler = new ContributionRequestHandler();
        private const int ConversationId = 1;

        private IRepository<Conversation> ConversationRepository
        {
            get { return (IRepository<Conversation>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>(); }
        }

        [TestFixture]
        public class HandleMessageTest : ContributionRequestHandlerTest
        {
            [Test]
            public void AddsNewContributionToConversation()
            {
                HandleMessage(contributionRequest);
                Conversation contributionConversation = ConversationRepository.FindEntityById(contributionRequest.Contribution.ConversationId);
                Contribution lastContributionAdded = contributionConversation.LastContribution;

                Assert.IsTrue(lastContributionAdded.Message.Equals(contributionRequest.Contribution.Message));
            }

            [Test]
            public void NewContributionGetsAssignedId()
            {
                HandleMessage(contributionRequest);
                Conversation contributionConversation = ConversationRepository.FindEntityById(contributionRequest.Contribution.ConversationId);
                Contribution lastContributionAdded = contributionConversation.LastContribution;

                Assert.IsTrue(lastContributionAdded.Id != 0);
            }

            [Test]
            public void ThrowsExceptionWhenNotGivenContributionRequest()
            {
                Assert.Throws<InvalidCastException>(() => HandleMessage(new ClientDisconnection(9)));
            }
        }
    }
}