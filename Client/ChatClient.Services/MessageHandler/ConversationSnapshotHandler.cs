using System;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationSnapshot"/> the Client received.
    /// </summary>
    internal sealed class ConversationSnapshotHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var conversationSnapshot = (ConversationSnapshot) message;

            context.RepositoryManager.ConversationRepository.AddConversations(conversationSnapshot.Conversations);

            OnConversationBootstrapCompleted();
        }

        public event EventHandler ConversationBootstrapCompleted;

        private void OnConversationBootstrapCompleted()
        {
            EventHandler handler = ConversationBootstrapCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}