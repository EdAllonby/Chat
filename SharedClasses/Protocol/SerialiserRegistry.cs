using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// This class defines various relationships of message types, identifiers and serialisers
    /// </summary>
    public static class SerialiserRegistry
    {
        public static readonly Dictionary<Type, int> IdentifiersByMessageType = new Dictionary<Type, int>
        {
            {typeof (Contribution), 0},
            {typeof (ContributionRequest), 1},
            {typeof (ContributionNotification), 2},
            {typeof (LoginRequest), 3}
        };

        public static Dictionary<int, Type> MessageTypesByIdentifiers = new Dictionary<int, Type>
        {
            {0, typeof(Contribution)},
            {1, typeof (ContributionRequest)},
            {2, typeof (ContributionNotification)},
            {3, typeof (LoginRequest)}
        };

        public static Dictionary<int, ISerialiser> serialisersByMessageNumber = new Dictionary<int, ISerialiser>
        {
            {0, new ContributionSerialiser()},
            {1, new ContributionRequestSerialiser()},
            {2, new ContributionNotificationSerialiser()},
            {3, new LoginRequestSerialiser()}
        };

        public static Dictionary<Type, ISerialiser> serialisersByMessageType = new Dictionary<Type, ISerialiser>
        {
            {typeof(Contribution), new ContributionSerialiser()},
            {typeof (ContributionRequest), new ContributionRequestSerialiser()},
            {typeof (ContributionNotification), new ContributionNotificationSerialiser()},
            {typeof (LoginRequest), new LoginRequestSerialiser()}
        };
    }
}