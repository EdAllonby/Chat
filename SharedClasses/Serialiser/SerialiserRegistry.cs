using System;
using System.Collections.Generic;
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
        /// A readonly version of an ISerialiser by Message Identifier dictionary. No one can alter this dictionary after compiling.
        /// </summary>
        public static readonly IReadOnlyDictionary<MessageIdentifier, ISerialiser> SerialisersByMessageIdentifier =
            new Dictionary<MessageIdentifier, ISerialiser>
            {
                {MessageIdentifier.ContributionRequest, new ContributionRequestSerialiser()},
                {MessageIdentifier.ContributionNotification, new ContributionNotificationSerialiser()},
                {MessageIdentifier.LoginRequest, new LoginRequestSerialiser()},
                {MessageIdentifier.UserNotification, new UserNotificationSerialiser()},
                {MessageIdentifier.UserSnapshotRequest, new UserSnapshotRequestSerialiser()},
                {MessageIdentifier.UserSnapshot, new UserSnapshotSerialiser()},
                {MessageIdentifier.ConversationSnapshotRequest, new ConversationSnapshotRequestSerialiser()},
                {MessageIdentifier.ConversationSnapshot, new ConversationSnapshotSerialiser()},
                {MessageIdentifier.ParticipationSnapshotRequest, new ParticipationSnapshotRequestSerialiser()},
                {MessageIdentifier.ParticipationSnapshot, new ParticipationSnapshotSerialiser()},
                {MessageIdentifier.NewConversationRequest, new ConversationRequestSerialiser()},
                {MessageIdentifier.ConversationNotification, new ConversationNotificationSerialiser()},
                {MessageIdentifier.ParticipationRequest, new ParticipationRequestSerialiser()},
                {MessageIdentifier.ParticipationNotification, new ParticipationNotificationSerialiser()},
                {MessageIdentifier.LoginResponse, new LoginResponseSerialiser()}
            };

        /// <summary>
        /// A readonly version of an ISerialiser by Message Type dictionary. No one can alter this dictionary after compiling.
        /// </summary>
        public static readonly IReadOnlyDictionary<Type, ISerialiser> SerialisersByMessageType =
            new Dictionary<Type, ISerialiser>
            {
                {typeof (ContributionRequest), new ContributionRequestSerialiser()},
                {typeof (ContributionNotification), new ContributionNotificationSerialiser()},
                {typeof (LoginRequest), new LoginRequestSerialiser()},
                {typeof (UserNotification), new UserNotificationSerialiser()},
                {typeof (UserSnapshotRequest), new UserSnapshotRequestSerialiser()},
                {typeof (UserSnapshot), new UserSnapshotSerialiser()},
                {typeof (ConversationSnapshotRequest), new ConversationSnapshotRequestSerialiser()},
                {typeof (ConversationSnapshot), new ConversationSnapshotSerialiser()},
                {typeof (ParticipationSnapshotRequest), new ParticipationSnapshotRequestSerialiser()},
                {typeof (ParticipationSnapshot), new ParticipationSnapshotSerialiser()},
                {typeof (NewConversationRequest), new ConversationRequestSerialiser()},
                {typeof (ConversationNotification), new ConversationNotificationSerialiser()},
                {typeof (ParticipationRequest), new ParticipationRequestSerialiser()},
                {typeof (ParticipationNotification), new ParticipationNotificationSerialiser()},
                {typeof (LoginResponse), new LoginResponseSerialiser()}
            };
    }
}