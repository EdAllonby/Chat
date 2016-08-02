using System;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ConversationSnapshot" /> the Client received.
    /// </summary>
    internal sealed class ConversationSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var conversationSnapshot = (ConversationSnapshot) message;

            var conversationRepository = (ConversationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            foreach (Conversation conversation in conversationSnapshot.Conversations)
            {
                conversationRepository.AddEntity(conversation);
            }

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