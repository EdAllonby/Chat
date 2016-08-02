using System;
using System.Drawing;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Models an image contribution that is part of a conversation.
    /// </summary>
    [Serializable]
    public sealed class ImageContribution : IContribution
    {
        public ImageContribution(int contributorUserId, Image image, int conversationId)
        {
            ContributorUserId = contributorUserId;
            Image = image;
            ConversationId = conversationId;
        }

        /// <summary>
        /// Creates a complete text contribution entity.
        /// </summary>
        /// <param name="id">The unique Id of the contribution entity.</param>
        /// <param name="incompleteContribution">The extra details of the <see cref="TextContribution" />.</param>
        public ImageContribution(int id, ImageContribution incompleteContribution)
            : this(incompleteContribution.ContributorUserId, incompleteContribution.Image, incompleteContribution.ConversationId)
        {
            Id = id;
            ContributionTimeStamp = DateTime.Now;
        }

        public Image Image { get; }

        public int Id { get; }

        public int ConversationId { get; }

        public int ContributorUserId { get; }

        public DateTime ContributionTimeStamp { get; }

        public ContributionType ContributionType => ContributionType.Image;

        public bool Equals(TextContribution other)
        {
            return Id == other.Id;
        }
    }
}