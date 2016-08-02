using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses.Message
{
    /// <summary>
    /// Gets the correct <see cref="MessageIdentifier" /> for the notification type.
    /// </summary>
    public sealed class NotificationMessageIdentifierFactory
    {
        /// <summary>
        /// A read only version of a <see cref="MessageIdentifier" /> by Entity
        /// <see cref="Type" /> dictionary. No one can alter this dictionary after compiling.
        /// </summary>
        private static readonly IReadOnlyDictionary<Type, MessageIdentifier> SerialisersByMessageType =
            new Dictionary<Type, MessageIdentifier>
            {
                { typeof(User), MessageIdentifier.UserNotification },
                { typeof(Participation), MessageIdentifier.ParticipationNotification },
                { typeof(Avatar), MessageIdentifier.AvatarNotification },
                { typeof(Conversation), MessageIdentifier.ConversationNotification },
                { typeof(IContribution), MessageIdentifier.ContributionNotification },
                { typeof(UserTyping), MessageIdentifier.UserTypingNotification },
                { typeof(ConnectionStatus), MessageIdentifier.ConnectionStatusNotification }
            };

        /// <summary>
        /// Returns the correct <see cref="MessageIdentifier" /> from the notification type.
        /// </summary>
        /// <param name="notificationType">The snapshot type to get the correct <see cref="MessageIdentifier" />.</param>
        /// <returns>The <see cref="MessageIdentifier" /> linked to the Notification Message Type.</returns>
        public MessageIdentifier GetIdentifierBySnapshotType(Type notificationType)
        {
            return SerialisersByMessageType[notificationType];
        }
    }
}