using System;
using System.Collections.Generic;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Defines various relationships of message types, identifiers and serialisers
    /// </summary>
    public static class SerialiserRegistry
    {
        public static readonly Dictionary<Type, int> IdentifiersByMessageType = new Dictionary<Type, int>
        {
            {typeof (ContributionRequest), MessageNumber.ContributionRequest},
            {typeof (ContributionNotification), MessageNumber.ContributionNotification},
            {typeof (LoginRequest), MessageNumber.LoginRequest},
            {typeof (LoginResponse), MessageNumber.LoginResponse},
            {typeof (UserNotification), MessageNumber.UserNotification},
            {typeof (UserSnapshotRequest), MessageNumber.UserSnapshotRequest},
            {typeof (UserSnapshot), MessageNumber.UserSnapshot}
        };

        public static readonly Dictionary<int, Type> MessageTypesByIdentifiers = new Dictionary<int, Type>
        {
            {MessageNumber.ContributionRequest, typeof (ContributionRequest)},
            {MessageNumber.ContributionNotification, typeof (ContributionNotification)},
            {MessageNumber.LoginRequest, typeof (LoginRequest)},
            {MessageNumber.LoginResponse, typeof (LoginResponse)},
            {MessageNumber.UserNotification, typeof (UserNotification)},
            {MessageNumber.UserSnapshotRequest, typeof (UserSnapshotRequest)},
            {MessageNumber.UserSnapshot, typeof (UserSnapshot)}
        };

        private static readonly Dictionary<int, ISerialiser> SerialisersByMessageIdentifier = new Dictionary<int, ISerialiser>
        {
            {MessageNumber.ContributionRequest, new ContributionRequestSerialiser()},
            {MessageNumber.ContributionNotification, new ContributionNotificationSerialiser()},
            {MessageNumber.LoginRequest, new LoginRequestSerialiser()},
            {MessageNumber.LoginResponse, new LoginResponseSerialiser()},
            {MessageNumber.UserNotification, new UserNotificationSerialiser()},
            {MessageNumber.UserSnapshotRequest, new UserSnapshotRequestSerialiser()},
            {MessageNumber.UserSnapshot, new UserSnapshotSerialiser()}
        };

        public static readonly Dictionary<Type, ISerialiser> SerialisersByMessageType = new Dictionary<Type, ISerialiser>
        {
            {typeof (ContributionRequest), new ContributionRequestSerialiser()},
            {typeof (ContributionNotification), new ContributionNotificationSerialiser()},
            {typeof (LoginRequest), new LoginRequestSerialiser()},
            {typeof (LoginResponse), new LoginResponseSerialiser()},
            {typeof (UserNotification), new UserNotificationSerialiser()},
            {typeof (UserSnapshotRequest), new UserSnapshotRequestSerialiser()},
            {typeof (UserSnapshot), new UserSnapshotSerialiser()}
        };

        public static ISerialiser GetSerialisersByMessageIdentifier(int identifier)
        {
            ISerialiser serialiser;
            bool keyExists = SerialisersByMessageIdentifier.TryGetValue(identifier, out serialiser);

            if (!keyExists)
            {
                throw new KeyNotFoundException("Serialiser not found for message identifier: " + identifier);
            }

            return serialiser;
        }
    }
}