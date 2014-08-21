using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ContributionNotification"/> the Client received.
    /// </summary>
    internal sealed class ContributionNotificationHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var contributionNotification = (ContributionNotification) message;

            ConversationRepository conversationRepository = (ConversationRepository) context.RepositoryManager.GetRepository<Conversation>();

            conversationRepository.AddContributionToConversation(contributionNotification.Contribution);
        }
    }
}