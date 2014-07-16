using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Gives a user a current connection status.
    /// </summary>
    [Serializable]
    public sealed class ConnectionStatus
    {
        public enum Status
        {
            Unknown,
            Connected,
            Disconnected
        }

        private readonly Status userConnectionStatus;
        private readonly int userId;

        public ConnectionStatus(int userId, Status userConnectionStatus)
        {
            this.userId = userId;
            this.userConnectionStatus = userConnectionStatus;
        }

        public int UserId
        {
            get { return userId; }
        }

        public Status UserConnectionStatus
        {
            get { return userConnectionStatus; }
        }
    }
}