using System.Diagnostics.Contracts;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Holds a collection of <see cref="Conversation"/>s with basic CRUD operations.
    /// </summary>
    public sealed class ConversationRepository : EntityRepository<Conversation>
    {
        /// <summary>
        /// Adds a contribution to a current conversation in the repository.
        /// </summary>
        /// <param name="contribution"></param>
        public void AddContributionToConversation(IContribution contribution)
        {
            Contract.Requires(contribution != null);
            Contract.Requires(contribution.Id > 0);
            Contract.Requires(contribution.ConversationId > 0);

            Conversation conversation = FindEntityById(contribution.ConversationId);
            Conversation previousConversation = conversation.CreateLightweightCopy();

            conversation.AddContribution(contribution);

            OnEntityUpdated(conversation, previousConversation);
        }
    }
}