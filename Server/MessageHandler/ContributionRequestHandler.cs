using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ContributionRequest" /> the Server received.
    /// </summary>
    internal sealed class ContributionRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var contributionRequest = (ContributionRequest) message;

            var conversationRepository = (ConversationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            var entityIdAllocatorFactory = serviceRegistry.GetService<EntityIdAllocatorFactory>();

            IContribution newContribution;

            IContribution contribution = contributionRequest.Contribution;

            switch (contribution.ContributionType)
            {
                case ContributionType.Text:
                    newContribution = new TextContribution(entityIdAllocatorFactory.AllocateEntityId<IContribution>(), (TextContribution) contribution);
                    conversationRepository.AddContributionToConversation(newContribution);
                    break;
                case ContributionType.Image:
                    newContribution = new ImageContribution(entityIdAllocatorFactory.AllocateEntityId<IContribution>(), (ImageContribution) contribution);
                    conversationRepository.AddContributionToConversation(newContribution);
                    break;
            }
        }
    }
}