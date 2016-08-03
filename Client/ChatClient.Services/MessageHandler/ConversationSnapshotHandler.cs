using System;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntitySnapshot{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ConversationSnapshotHandler : MessageHandler<EntitySnapshot<Conversation>>, IBootstrapper
    {
        public ConversationSnapshotHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        public event EventHandler<EntityBootstrapEventArgs> EntityBootstrapCompleted;

        protected override void HandleMessage(EntitySnapshot<Conversation> message)
        {
            var conversationRepository = (ConversationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

            foreach (Conversation conversation in message.Entities)
            {
                conversationRepository.AddEntity(conversation);
            }

            OnConversationBootstrapCompleted();
        }

        private void OnConversationBootstrapCompleted()
        {
            EventHandler<EntityBootstrapEventArgs> handler = EntityBootstrapCompleted;
            handler?.Invoke(this, new EntityBootstrapEventArgs(typeof(Conversation)));
        }
    }
}