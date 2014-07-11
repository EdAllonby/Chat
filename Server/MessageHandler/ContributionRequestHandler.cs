using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ContributionRequest"/> the Server received.
    /// </summary>
    internal sealed class ContributionRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var contributionRequest = (ContributionRequest) message;
            var contributionRequestContext = (ContributionRequestContext) context;

            CreateContributionEntity(contributionRequest, contributionRequestContext);
        }

        private static void CreateContributionEntity(ContributionRequest contributionRequest,
            ContributionRequestContext contributionRequestContext)
        {
            var newContribution = new Contribution(
                contributionRequestContext.EntityIdAllocatorFactory.AllocateEntityId<Contribution>(),
                contributionRequest.Contribution);

            contributionRequestContext.ConversationRepository.AddContributionToConversation(newContribution);
        }
    }
}