using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ContributionNotificationHandler"/> needs. 
    /// </summary>
    internal sealed class ContributionNotificationContext : IMessageContext
    {
        private readonly ConversationRepository conversationRepository;

        public ContributionNotificationContext(ConversationRepository conversationRepository)
        {
            this.conversationRepository = conversationRepository;
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }
    }
}