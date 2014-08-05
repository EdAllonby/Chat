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

            AddContributionToConversation(contributionNotification, context);
        }

        private static void AddContributionToConversation(ContributionNotification contributionNotification,
            IClientMessageContext context)
        {
            context.RepositoryManager.ConversationRepository
                .AddContributionToConversation(contributionNotification.Contribution);
        }
    }
}