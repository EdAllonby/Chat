using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationSnapshot"/> the Server received.
    /// </summary>
    internal sealed class ConversationSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            ConversationSnapshot conversationSnapshot = (ConversationSnapshot) message;
            ConversationSnapshotContext conversationSnapshotContext = (ConversationSnapshotContext) context;

            conversationSnapshotContext.ConversationRepository.AddConversations(conversationSnapshot.Conversations);
        }
    }
}