namespace SharedClasses.Protocol
{
    /// <summary>
    /// Gives definitions for the numbers, avoiding the introduction of Magic Numbers
    /// </summary>
    public static class MessageNumber
    {
        public const int ContributionRequest = 1;
        public const int ContributionNotification = 2;
        public const int LoginRequest = 3;
        public const int UserNotification = 4;
        public const int UserSnapshotRequest = 5;
        public const int UserSnapshot = 6;
        public const int ClientDisconnection = 7;
    }
}