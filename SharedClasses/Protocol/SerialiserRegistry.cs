using System;
using System.Collections.Generic;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Defines various relationships of message types, identifiers and serialisers
    /// </summary>
    public static class SerialiserRegistry
    {
        public static readonly Dictionary<int, ISerialiser> SerialisersByMessageIdentifier = new Dictionary<int, ISerialiser>
        {
            {MessageNumber.ContributionRequest, new ContributionRequestSerialiser()},
            {MessageNumber.ContributionNotification, new ContributionNotificationSerialiser()},
            {MessageNumber.LoginRequest, new LoginRequestSerialiser()},
            {MessageNumber.LoginResponse, new LoginResponseSerialiser()},
            {MessageNumber.UserNotification, new UserNotificationSerialiser()},
            {MessageNumber.UserSnapshotRequest, new UserSnapshotRequestSerialiser()},
            {MessageNumber.UserSnapshot, new UserSnapshotSerialiser()},
            {MessageNumber.ConversationRequest, new ConversationRequestSerialiser()},
            {MessageNumber.ConversationNotification, new ConversationNotificationSerialiser()}
        };

        public static readonly Dictionary<Type, ISerialiser> SerialisersByMessageType = new Dictionary<Type, ISerialiser>
        {
            {typeof (ContributionRequest), new ContributionRequestSerialiser()},
            {typeof (ContributionNotification), new ContributionNotificationSerialiser()},
            {typeof (LoginRequest), new LoginRequestSerialiser()},
            {typeof (LoginResponse), new LoginResponseSerialiser()},
            {typeof (UserNotification), new UserNotificationSerialiser()},
            {typeof (UserSnapshotRequest), new UserSnapshotRequestSerialiser()},
            {typeof (UserSnapshot), new UserSnapshotSerialiser()},
            {typeof (ConversationRequest), new ConversationRequestSerialiser()},
            {typeof (ConversationNotification), new ConversationNotificationSerialiser()}
        };
    }
}