namespace SharedClasses
{
    /// <summary>
    /// Gives definitions for the numbers, avoiding the introduction of Magic Numbers
    /// </summary>
    public enum MessageNumber
    {
        UnrecognisedMessage = 0, // in place to safeguard against unassigned ints.
        ContributionRequest,
        ContributionNotification,
        LoginRequest,
        UserNotification,
        UserSnapshotRequest,
        UserSnapshot,
        ClientDisconnection,
        ConversationRequest,
        ConversationNotification,
        LoginResponse,
    }
}