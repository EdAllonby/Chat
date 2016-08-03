using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ContributionNotificationHandler : MessageHandler<EntityNotification<IContribution>>
    {
        public ContributionNotificationHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntityNotification<IContribution> message)
        {
            var conversationRepository = (ConversationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            conversationRepository.AddContributionToConversation(message.Entity);
        }
    }
}