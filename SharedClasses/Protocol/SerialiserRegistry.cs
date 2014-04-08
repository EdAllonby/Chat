using System;
using System.Collections.Generic;
using log4net;

namespace SharedClasses.Protocol
{
    /// <summary>
    ///     Defines various relationships of message types, identifiers and serialisers
    /// </summary>
    public static class SerialiserRegistry
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SerialiserRegistry));

        public static readonly Dictionary<Type, int> IdentifiersByMessageType = new Dictionary<Type, int>
        {
            {typeof (ContributionRequest), 1},
            {typeof (ContributionNotification), 2},
            {typeof (LoginRequest), 3},
            {typeof (UserNotification), 4},
            {typeof (UserSnapshotRequest), 5},
            {typeof (UserSnapshot), 6}
        };

        public static readonly Dictionary<int, Type> MessageTypesByIdentifiers = new Dictionary<int, Type>
        {
            {1, typeof (ContributionRequest)},
            {2, typeof (ContributionNotification)},
            {3, typeof (LoginRequest)},
            {4, typeof (UserNotification)},
            {5, typeof (UserSnapshotRequest)},
            {6, typeof (UserSnapshot)}
        };

        private static readonly Dictionary<int, ISerialiser> SerialisersByMessageIdentifier = new Dictionary <int, ISerialiser>
        {
            {1, new ContributionRequestSerialiser()},
            {2, new ContributionNotificationSerialiser()},
            {3, new LoginRequestSerialiser()},
            {4, new UserNotificationSerialiser()},
            {5, new UserSnapshotRequestSerialiser()},
            {6, new UserSnapshotSerialiser()}
        };

        public static readonly Dictionary<Type, ISerialiser> SerialisersByMessageType = new Dictionary <Type, ISerialiser>
        {
            {typeof (ContributionRequest), new ContributionRequestSerialiser()},
            {typeof (ContributionNotification), new ContributionNotificationSerialiser()},
            {typeof (LoginRequest), new LoginRequestSerialiser()},
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
                Log.Error("Serialiser not found for message identifier: " + identifier);
                throw new KeyNotFoundException();
            }

            return serialiser;
        }
    }
}