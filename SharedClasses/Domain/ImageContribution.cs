using System;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Models an image contribution that is part of a conversation.
    /// </summary>
    [Serializable]
    public sealed class ImageContribution : IContribution
    {
        private readonly DateTime contributionTimeStamp;
        private readonly int contributorUserId;
        private readonly int conversationId;
        private readonly int id;
        private readonly Image image;

        public ImageContribution(int contributorUserId, Image image, int conversationId)
        {
            Contract.Requires(contributorUserId > 0);
            Contract.Requires(conversationId > 0);

            this.contributorUserId = contributorUserId;
            this.image = image;
            this.conversationId = conversationId;
        }

        /// <summary>
        /// Creates a complete text contribution entity.
        /// </summary>
        /// <param name="id">The unique Id of the contribution entity.</param>
        /// <param name="incompleteContribution">The extra details of the <see cref="TextContribution"/>.</param>
        public ImageContribution(int id, ImageContribution incompleteContribution)
            : this(incompleteContribution.ContributorUserId, incompleteContribution.Image, incompleteContribution.ConversationId)
        {
            Contract.Requires(id > 0);
            Contract.Requires(incompleteContribution != null);

            this.id = id;
            contributionTimeStamp = DateTime.Now;
        }

        public Image Image
        {
            get { return image; }
        }

        public int Id
        {
            get { return id; }
        }

        public int ConversationId
        {
            get { return conversationId; }
        }

        public int ContributorUserId
        {
            get { return contributorUserId; }
        }

        public DateTime ContributionTimeStamp
        {
            get { return contributionTimeStamp; }
        }

        public ContributionType ContributionType
        {
            get { return ContributionType.Image; }
        }

        public bool Equals(TextContribution other)
        {
            return Id == other.Id;
        }
    }
}