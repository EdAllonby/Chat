using System;

namespace SharedClasses.Domain
{
    /// <summary>
    /// Gives a user a current connection status.
    /// </summary>
    [Serializable]
    public sealed class ConnectionStatus : IEntity
    {
        public enum Status
        {
            /// <summary>
            /// The current status of the user is not known, indicating an error.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// The user is currently connected and is able to send to and receive messages from the server.
            /// </summary>
            Connected,

            /// <summary>
            /// The user is disconnected from the server. They will be unable to send and receive messages.
            /// Other clients will see the Client as disconnected and cannot send messages directly to them.
            /// </summary>
            Disconnected
        }

        public ConnectionStatus(int userId, Status userConnectionStatus)
        {
            UserId = userId;
            UserConnectionStatus = userConnectionStatus;
        }

        public int UserId { get; }

        /// <summary>
        /// The current status of the user.
        /// </summary>
        public Status UserConnectionStatus { get; }

        public int Id { get; } = 1;
    }
}