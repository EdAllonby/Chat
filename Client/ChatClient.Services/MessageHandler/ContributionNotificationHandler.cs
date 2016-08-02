using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ContributionNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var contributionNotification = (EntityNotification<IContribution>) message;

            var conversationRepository = (ConversationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            conversationRepository.AddContributionToConversation(contributionNotification.Entity);
        }
    }
}