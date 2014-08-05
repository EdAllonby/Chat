using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ContributionRequest"/> the Server received.
    /// </summary>
    internal sealed class ContributionRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var contributionRequest = (ContributionRequest) message;

            var newContribution = new Contribution(
                context.EntityIdAllocatorFactory.AllocateEntityId<Contribution>(),
                contributionRequest.Contribution);

            context.RepositoryManager.ConversationRepository.AddContributionToConversation(newContribution);
        }
    }
}