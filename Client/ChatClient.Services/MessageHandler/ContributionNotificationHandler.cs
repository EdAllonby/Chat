using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ContributionNotification" /> the Client received.
    /// </summary>
    internal sealed class ContributionNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var contributionNotification = (ContributionNotification) message;

            var conversationRepository = (ConversationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            conversationRepository.AddContributionToConversation(contributionNotification.Contribution);
        }
    }
}