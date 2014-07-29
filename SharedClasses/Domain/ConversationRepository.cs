using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Holds a collection of <see cref="Conversation"/>s with basic CRUD operations.
    /// </summary>
    public sealed class ConversationRepository : Repository<Conversation>
    {
        /// <summary>
        /// Adds a <see cref="Conversation"/> entity to the repository.
        /// </summary>
        /// <param name="conversation"><see cref="Conversation"/> entity to add.</param>
        public void AddConversation(Conversation conversation)
        {
            AddEntity(conversation);
        }

        public void UpdateConversation(Conversation conversation)
        {
            Contract.Requires(conversation != null);

            Conversation previousConversation = Conversation.DeepClone(EntitiesIndexedById[conversation.Id]);

            EntitiesIndexedById[conversation.Id] = conversation;

            var conversationChangedEventArgs = new EntityChangedEventArgs<Conversation>();
            conversationChangedEventArgs.EntityUpdated(conversation, previousConversation);
            OnEntityChanged(conversationChangedEventArgs);

            Log.DebugFormat("Conversation with Id {0} has been updated.", conversation.Id);
        }

        public void AddContributionToConversation(Contribution contribution)
        {
            Contract.Requires(contribution != null);
            Contract.Requires(contribution.Id > 0);
            Contract.Requires(contribution.ConversationId > 0);

            Conversation conversation = FindEntityById(contribution.ConversationId);
            Conversation previousConversation = Conversation.DeepClone(conversation);

            conversation.AddContribution(contribution);

            var conversationChangedEventArgs = new EntityChangedEventArgs<Conversation>();
            conversationChangedEventArgs.EntityUpdated(conversation, previousConversation);

            OnEntityChanged(conversationChangedEventArgs);
        }

        /// <summary>
        /// Adds <see cref="Conversation"/> entities to the repository.
        /// </summary>
        /// <param name="conversations"><see cref="Conversation"/> entities to add to the repository.</param>
        public void AddConversations(IEnumerable<Conversation> conversations)
        {
            Contract.Requires(conversations != null);

            IEnumerable<Conversation> conversationsEnumerable = conversations as IList<Conversation> ??
                                                                conversations.ToList();
            foreach (Conversation conversation in conversationsEnumerable)
            {
                EntitiesIndexedById[conversation.Id] = conversation;
                Log.Debug("Conversation with Id " + conversation.Id + " added to conversation repository");
            }
        }
    }
}