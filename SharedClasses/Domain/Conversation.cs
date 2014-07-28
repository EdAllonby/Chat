using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SharedClasses.Message;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Holds a collection of <see cref="Contribution"/>s that are linked by a Conversation
    /// </summary>
    [Serializable]
    public sealed class Conversation : IEntity, IEquatable<Conversation>
    {
        private readonly Dictionary<int, Contribution> contributionsIndexedByContributionID = new Dictionary<int, Contribution>();

        private readonly int id;

        /// <summary>
        /// Creates a conversation entity with conversation Id.
        /// </summary>
        /// <param name="id">The Id to assign to the conversation.</param>
        public Conversation(int id)
        {
            Contract.Requires(id > 0);

            this.id = id;
        }

        /// <summary>
        /// Conversation is a domain entity class and gets a unique Id.
        /// </summary>
        public int Id
        {
            get { return id; }
        }

        public bool Equals(Conversation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return id == other.id;
        }

        /// <summary>
        /// Adds a <see cref="Contribution"/> entity to the dictionary indexed by ids.
        /// </summary>
        public void AddContribution(Contribution newContribution)
        {
            Contract.Requires(newContribution != null);
            Contract.Requires(newContribution.ConversationId == Id);

            contributionsIndexedByContributionID[newContribution.Id] = newContribution;
        }

        /// <summary>
        /// Adds a <see cref="Contribution"/> from an incoming <see cref="ContributionNotification"/>.
        /// The <see cref="ContributionNotification"/> must have an ID otherwise it is not following the protocol.
        /// </summary>
        /// <param name="contributionNotification">The contribution to add to the conversation packaged in a <see cref="ContributionNotification"/>.</param>
        public void AddContribution(ContributionNotification contributionNotification)
        {
            Contract.Requires(contributionNotification != null);

            contributionsIndexedByContributionID[contributionNotification.Contribution.Id] = contributionNotification.Contribution;
        }

        /// <summary>
        /// Returns a list of <see cref="Contribution"/>s which are held in this <see cref="Conversation"/> entity.
        /// </summary>
        /// <returns>A collection of all contributions associated with the conversation.</returns>
        public IEnumerable<Contribution> GetAllContributions()
        {
            return new List<Contribution>(contributionsIndexedByContributionID.Values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Conversation && Equals((Conversation) obj);
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}