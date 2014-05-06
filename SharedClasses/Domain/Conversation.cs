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

        /// <summary>
        /// Adds a <see cref="Contribution"/> from a new <see cref="ContributionRequest"/> 
        /// First assigns a Contribution Id and then adds this to the contribution repository.
        /// </summary>
        /// <param name="contributionRequest"></param>
        /// <returns></returns>
        public Contribution CreateContributionEntity(ContributionRequest contributionRequest)
        {
            var contribution = new Contribution(nextContributionId, contributionRequest.Contribution);

            contributionsIndexedByContributionID[contribution.ContributionId] = contribution;

            nextContributionId++;

            return contribution;
        }

        /// <summary>
        /// Adds a <see cref="Contribution"/> from an incoming <see cref="ContributionNotification"/>
        /// The <see cref="ContributionNotification"/> must have an ID otherwise it is not following the protocol
        /// </summary>
        /// <param name="contributionNotification"></param>
        public void AddContribution(ContributionNotification contributionNotification)
        {
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