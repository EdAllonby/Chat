using System;
using System.Collections.Generic;
using SharedClasses.Domain;
using SharedClasses.Message;
using SharedClasses.Serialiser.MessageSerialiser;

namespace SharedClasses.Serialiser
{
    /// <summary>
    /// Defines various relationships of message types, identifiers and serialisers
    /// </summary>
    public static class SerialiserRegistry
    {
        /// <summary>
        /// A readonly version of an ISerialiser by Message Identifier dictionary. No one can alter this dictionary after
        /// compiling.
        /// </summary>
        public static readonly IReadOnlyDictionary<MessageIdentifier, IMessageSerialiser> SerialisersByMessageIdentifier =
            new Dictionary<MessageIdentifier, IMessageSerialiser>
            {
                { MessageIdentifier.ContributionRequest, new MessageSerialiser<ContributionRequest>() },
                { MessageIdentifier.ContributionNotification, new EntityNotificationSerialiser<IContribution>() },
                { MessageIdentifier.LoginRequest, new MessageSerialiser<LoginRequest>() },
                { MessageIdentifier.UserNotification, new EntityNotificationSerialiser<User>() },
                { MessageIdentifier.UserSnapshotRequest, new MessageSerialiser<UserSnapshotRequest>() },
                { MessageIdentifier.UserSnapshot, new MessageSerialiser<UserSnapshot>() },
                { MessageIdentifier.ConversationSnapshotRequest, new MessageSerialiser<ConversationSnapshotRequest>() },
                { MessageIdentifier.ConversationSnapshot, new MessageSerialiser<ConversationSnapshot>() },
                { MessageIdentifier.ParticipationSnapshotRequest, new MessageSerialiser<ParticipationSnapshotRequest>() },
                { MessageIdentifier.ParticipationSnapshot, new MessageSerialiser<ParticipationSnapshot>() },
                { MessageIdentifier.ConversationRequest, new MessageSerialiser<ConversationRequest>() },
                { MessageIdentifier.ConversationNotification, new EntityNotificationSerialiser<Conversation>() },
                { MessageIdentifier.ParticipationRequest, new MessageSerialiser<ParticipationRequest>() },
                { MessageIdentifier.ParticipationNotification, new EntityNotificationSerialiser<Participation>() },
                { MessageIdentifier.LoginResponse, new MessageSerialiser<LoginResponse>() },
                { MessageIdentifier.AvatarRequest, new MessageSerialiser<AvatarRequest>() },
                { MessageIdentifier.AvatarNotification, new EntityNotificationSerialiser<Avatar>() },
                { MessageIdentifier.ConnectionStatusNotification, new EntityNotificationSerialiser<ConnectionStatus>() },
                { MessageIdentifier.UserTypingRequest, new MessageSerialiser<UserTypingRequest>() },
                { MessageIdentifier.UserTypingNotification, new EntityNotificationSerialiser<UserTyping>() }
            };
    }
}