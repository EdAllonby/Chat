using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ContributionRequest" /> the Server received.
    /// </summary>
    internal sealed class ContributionRequestHandler : MessageHandler<ContributionRequest>
    {
        public ContributionRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(ContributionRequest message)
        {
            var conversationRepository = (ConversationRepository)ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            var entityIdAllocatorFactory = ServiceRegistry.GetService<EntityIdAllocatorFactory>();

            IContribution newContribution;

            IContribution contribution = message.Contribution;

            switch (contribution.ContributionType)
            {
                case ContributionType.Text:
                    newContribution = new TextContribution(entityIdAllocatorFactory.AllocateEntityId<IContribution>(), (TextContribution)contribution);
                    conversationRepository.AddContributionToConversation(newContribution);
                    break;
                case ContributionType.Image:
                    newContribution = new ImageContribution(entityIdAllocatorFactory.AllocateEntityId<IContribution>(), (ImageContribution)contribution);
                    conversationRepository.AddContributionToConversation(newContribution);
                    break;
            }
        }
    }
}