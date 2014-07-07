using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ConversationNotificationHandler"/> needs. 
    /// </summary>
    internal sealed class ConversationNotificationContext : IMessageContext
    {
        private readonly ConversationRepository conversationRepository;

        public ConversationNotificationContext(ConversationRepository conversationRepository)
        {
            this.conversationRepository = conversationRepository;
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }
    }
}