using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using log4net;

namespace SharedClasses.Domain
{
    public delegate void ConversationChangedHandler(Conversation conversation);

    public delegate void ContributionAddedHandler(Contribution contribution);

    /// <summary>
    /// Holds a collection of <see cref="Conversation"/>s with basic CRUD operations.
    /// </summary>
    public sealed class ConversationRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ConversationRepository));

        private readonly Dictionary<int, Conversation> conversationsIndexedById = new Dictionary<int, Conversation>();

        public event ConversationChangedHandler ConversationAdded = delegate { };

        public event ContributionAddedHandler ContributionAdded = delegate { };

        /// <summary>
        /// Adds a <see cref="Conversation"/> entity to the repository.
        /// </summary>
        /// <param name="conversation"><see cref="Conversation"/> entity to add.</param>
        public void AddConversation(Conversation conversation)
        {
            Contract.Requires(conversation != null);

            conversationsIndexedById[conversation.ConversationId] = conversation;
            Log.Debug("Conversation with Id " + conversation.ConversationId + " added to conversation repository");

            ConversationAdded(conversation);
        }

        public void AddContributionToConversation(Contribution contribution)
        {
            Contract.Requires(contribution != null);
            Contract.Requires(contribution.ContributionId > 0);
            Contract.Requires(contribution.ConversationId > 0);

            Conversation conversation = FindConversationById(contribution.ConversationId);

            conversation.AddContribution(contribution);

            ContributionAdded(contribution);
        }

        /// <summary>
        /// Adds <see cref="Conversation"/> entities to the repository.
        /// </summary>
        /// <param name="conversations"><see cref="Conversation"/> entities to add to the repository.</param>
        public void AddConversations(IEnumerable<Conversation> conversations)
        {
            Contract.Requires(conversations != null);

            IEnumerable<Conversation> conversationsEnumerable = conversations as IList<Conversation> ?? conversations.ToList();
            foreach (Conversation conversation in conversationsEnumerable)
            {
                conversationsIndexedById[conversation.ConversationId] = conversation;
                Log.Debug("Conversation with Id " + conversation.ConversationId + " added to conversation repository");
            }
        }

        /// <summary>
        /// Retrieves a <see cref="Conversation"/> entity from the repository.
        /// </summary>
        /// <param name="conversationID">The <see cref="Conversation"/> entity ID to find.</param>
        /// <returns>The <see cref="Conversation"/> which matches the ID. If no <see cref="Conversation"/> is found, return null.</returns>
        public Conversation FindConversationById(int conversationID)
        {
            return conversationsIndexedById.ContainsKey(conversationID) ? conversationsIndexedById[conversationID] : null;
        }

        /// <summary>
        /// Retrieves all <see cref="Conversation"/> entities from the repository.
        /// </summary>
        /// <returns>A collection of all <see cref="Conversation"/> entities in the repository.</returns>
        public IEnumerable<Conversation> GetAllConversations()
        {
            return conversationsIndexedById.Values;
        }
    }
}