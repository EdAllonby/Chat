using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// A fundamental domain entity object needed to model a type of contribution to a conversation.
    /// </summary>
    public interface IContribution : IEntity, IEquatable<TextContribution>
    {
        /// <summary>
        /// The Conversation Id this Contribution belongs to.
        /// </summary>
        int ConversationId { get; }

        /// <summary>
        /// The User who sent this Contribution message.
        /// </summary>
        int ContributorUserId { get; }

        /// <summary>
        /// The time the server received the message.
        /// </summary>
        DateTime ContributionTimeStamp { get; }

        ContributionType ContributionType { get; }
    }
}