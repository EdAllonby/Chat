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
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var contributionRequest = (ContributionRequest) message;

            ConversationRepository conversationRepository = (ConversationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            var entityIdAllocatorFactory = serviceRegistry.GetService<EntityIdAllocatorFactory>();

            var newContribution = new Contribution(entityIdAllocatorFactory.AllocateEntityId<Contribution>(), contributionRequest.Contribution);

            conversationRepository.AddContributionToConversation(newContribution);
        }
    }
}