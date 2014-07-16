using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ConversationSnapshotHandler"/> needs. 
    /// </summary>
    internal sealed class ConversationSnapshotContext : IMessageContext
    {
        private readonly ConversationRepository conversationRepository;

        public ConversationSnapshotContext(ConversationRepository conversationRepository)
        {
            this.conversationRepository = conversationRepository;
        }

        public ConversationRepository ConversationRepository
        {
            get { return conversationRepository; }
        }
    }
}