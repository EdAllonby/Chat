using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ContributionNotification"/> the Client received.
    /// </summary>
    internal sealed class ContributionNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var contributionNotification = (ContributionNotification) message;
            var contributionNotificationContext = (ContributionNotificationContext) context;

            AddContributionToConversation(contributionNotification, contributionNotificationContext);
        }

        private static void AddContributionToConversation(ContributionNotification contributionNotification,
            ContributionNotificationContext contributionNotificationContext)
        {
            contributionNotificationContext.ConversationRepository
                .AddContributionToConversation(contributionNotification.Contribution);
        }
    }
}