using SharedClasses.Message;

namespace SharedClasses
{
    /// <summary>
    /// Gives definitions for the types of <see cref="IMessage" />, avoiding the introduction of magic numbers.
    /// </summary>
    public enum MessageIdentifier
    {
        UnrecognisedMessage = 0, // in place to safeguard against unassigned ints.
        ContributionRequest,
        ContributionNotification,
        LoginRequest,
        UserNotification,
        UserSnapshotRequest,
        UserSnapshot,
        ConversationSnapshotRequest,
        ConversationSnapshot,
        ParticipationSnapshotRequest,
        ParticipationSnapshot,
        ClientDisconnection,
        ConversationRequest,
        ParticipationRequest,
        ParticipationNotification,
        LoginResponse,
        ConversationNotification,
        AvatarRequest,
        AvatarNotification,
        ConnectionStatusNotification,
        UserTypingRequest,
        UserTypingNotification
    }
}