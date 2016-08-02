using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedClasses.Domain
{
    /// <summary>
    /// A conversation holds <see cref="IContribution" />s that are linked to it.
    /// </summary>
    [Serializable]
    public sealed class Conversation : IEntity, IEquatable<Conversation>
    {
        private readonly Dictionary<int, IContribution> contributionsIndexedByContributionId = new Dictionary<int, IContribution>();

        /// <summary>
        /// Creates a conversation entity with conversation Id.
        /// </summary>
        /// <param name="id">The Id to assign to the conversation.</param>
        public Conversation(int id)
        {
            Id = id;
        }

        /// <summary>
        /// The most recent contribution added.
        /// </summary>
        public IContribution LastContribution => contributionsIndexedByContributionId.Values.LastOrDefault();

        /// <summary>
        /// Conversation is a domain entity class and gets a unique Id.
        /// </summary>
        public int Id { get; }

        public bool Equals(Conversation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        /// <summary>
        /// Creates a reduced size Conversation only containing Ids associated with it.
        /// </summary>
        /// <returns>A lightweight Conversation.</returns>
        public Conversation CreateLightweightCopy()
        {
            var conversation = new Conversation(Id);

            foreach (int contributionId in contributionsIndexedByContributionId.Keys)
            {
                conversation.contributionsIndexedByContributionId.Add(contributionId, null);
            }

            return conversation;
        }

        /// <summary>
        /// Adds an <see cref="IContribution" /> entity to this conversation.
        /// The contribution's conversation Id must match this conversation's Id.
        /// </summary>
        public void AddContribution(IContribution newContribution)
        {
            contributionsIndexedByContributionId.Add(newContribution.Id, newContribution);
        }

        /// <summary>
        /// Returns a list of <see cref="TextContribution" />s which are held in this <see cref="Conversation" /> entity.
        /// </summary>
        /// <returns>A collection of all contributions associated with the conversation.</returns>
        public IEnumerable<IContribution> GetAllContributions()
        {
            return new List<IContribution>(contributionsIndexedByContributionId.Values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Conversation && Equals((Conversation) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}