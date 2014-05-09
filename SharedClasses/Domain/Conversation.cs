using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using SharedClasses.Message;

namespace SharedClasses.Domain
{
    /// <summary>
    /// A link between two Users where they can both talk to each other privately.
    /// </summary>
    [Serializable]
    public sealed class Conversation
    {
        // Immutable domain entity class
        private readonly Dictionary<int, Contribution> contributionsIndexedByContributionID = new Dictionary<int, Contribution>();

        private readonly int conversationId;
        private readonly int firstParticipantUserId;
        private readonly int secondParticipantUserId;

        public Conversation(int conversationId, int firstParticipantUserId, int secondParticipantUserId)
        {
            Contract.Requires(conversationId > 0);

            this.conversationId = conversationId;
            this.firstParticipantUserId = firstParticipantUserId;
            this.secondParticipantUserId = secondParticipantUserId;
        }

        public Conversation(int firstParticipantUserId, int secondParticipantUserId)
        {
            this.firstParticipantUserId = firstParticipantUserId;
            this.secondParticipantUserId = secondParticipantUserId;
        }

        /// <summary>
        /// Conversation is a domain entity class and gets a unique ID
        /// </summary>
        public int ConversationId
        {
            get { return conversationId; }
        }

        /// <summary>
        /// The identity of the user that initiates the conversation
        /// </summary>
        public int FirstParticipantUserId
        {
            get { return firstParticipantUserId; }
        }

        /// <summary>
        /// The identity of the User who <see cref="FirstParticipantUserId"/> wants to talk to
        /// </summary>
        public int SecondParticipantUserId
        {
            get { return secondParticipantUserId; }
        }

        /// <summary>
        /// Adds a <see cref="Contribution"/> entity to the dictionary indexed by ids.
        /// </summary>
        /// <returns></returns>
        public void AddContribution(Contribution newContribution)
        {
            Contract.Requires(newContribution != null);
            Contract.Requires(newContribution.ContributionId > 0);

            contributionsIndexedByContributionID[newContribution.ContributionId] = newContribution;
        }

        /// <summary>
        /// Adds a <see cref="Contribution"/> from an incoming <see cref="ContributionNotification"/>
        /// The <see cref="ContributionNotification"/> must have an ID otherwise it is not following the protocol
        /// </summary>
        /// <param name="contributionNotification"></param>
        public void AddContribution(ContributionNotification contributionNotification)
        {
            Contract.Requires(contributionNotification != null);

            contributionsIndexedByContributionID[contributionNotification.Contribution.ContributionId] = contributionNotification.Contribution;
        }

        /// <summary>
        /// Returns a list of <see cref="Contribution"/>s which are held in this <see cref="Conversation"/> entity
        /// </summary>
        /// <returns></returns>
        public List<Contribution> GetAllContributions()
        {
            return new List<Contribution>(contributionsIndexedByContributionID.Values);
        }
    }
}