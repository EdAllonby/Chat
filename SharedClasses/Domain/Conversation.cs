using System;
using System.Collections.Generic;
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
        private int nextContributionId;

        public Conversation(int conversationId, int firstParticipantUserId, int secondParticipantUserId)
        {
            if (conversationId < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

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

        public Contribution AddContribution(ContributionRequest contribution)
        {
            var newContribution = new Contribution(nextContributionId, contribution.Contribution);

            contributionsIndexedByContributionID[newContribution.ContributionId] = newContribution;

            nextContributionId++;

            return newContribution;
        }

        public void AddContribution(Contribution contribution)
        {
            contributionsIndexedByContributionID[contribution.ContributionId] = contribution;
        }

        public List<Contribution> GetAllContributions()
        {
            return new List<Contribution>(contributionsIndexedByContributionID.Values);
        }
    }
}