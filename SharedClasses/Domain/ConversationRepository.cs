﻿using System.Diagnostics.Contracts;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Holds a collection of <see cref="Conversation"/>s with basic CRUD operations.
    /// </summary>
    public sealed class ConversationRepository : EntityRepository<Conversation>
    {
        public void UpdateConversation(Conversation conversation)
        {
            Contract.Requires(conversation != null);

            Conversation previousConversation = Conversation.DeepClone(FindEntityById(conversation.Id));

            OnEntityUpdated(conversation, previousConversation);

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

            OnEntityUpdated(conversation, previousConversation);
        }
    }
}