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
                { MessageIdentifier.UserSnapshotRequest, new MessageSerialiser<EntitySnapshotRequest<User>>() },
                { MessageIdentifier.UserSnapshot, new MessageSerialiser<EntitySnapshot<User>>() },
                { MessageIdentifier.ConversationSnapshotRequest, new MessageSerialiser<EntitySnapshotRequest<Conversation>>() },
                { MessageIdentifier.ConversationSnapshot, new MessageSerialiser<EntitySnapshot<Conversation>>() },
                { MessageIdentifier.ParticipationSnapshotRequest, new MessageSerialiser<EntitySnapshotRequest<Participation>>() },
                { MessageIdentifier.ParticipationSnapshot, new MessageSerialiser<EntitySnapshot<Participation>>() },
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