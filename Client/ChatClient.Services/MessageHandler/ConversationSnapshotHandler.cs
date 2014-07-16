using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationSnapshot"/> the Client received.
    /// </summary>
    internal sealed class ConversationSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var conversationSnapshot = (ConversationSnapshot) message;
            var conversationSnapshotContext = (ConversationSnapshotContext) context;

            conversationSnapshotContext.ConversationRepository.AddConversations(conversationSnapshot.Conversations);
        }
    }
}