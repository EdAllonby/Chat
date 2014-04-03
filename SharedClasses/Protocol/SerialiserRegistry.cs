using System;
using System.Collections.Generic;

namespace SharedClasses.Protocol
{
    public class SerialiserRegistry
    {
        public Dictionary<Type, ISerialiser> serialisersByMessageType = new Dictionary<Type, ISerialiser>
        {
            {typeof (ContributionRequest), new ContributionRequestSerialiser()},
            {typeof (ContributionNotification), new ContributionNotificationSerialiser()},
            {typeof (LoginRequest), new LoginRequestSerialiser()}
        };

        public Dictionary<int, ISerialiser> serialisersByMessageNumber = new Dictionary<int, ISerialiser>
        {
            {1, new ContributionRequestSerialiser()},
            {2, new ContributionNotificationSerialiser()},
            {3, new LoginRequestSerialiser()},
        };

        public static Dictionary<int, Type> MessageIdentifierRegistry = new Dictionary<int, Type>
        {
            {1, typeof (ContributionRequest)},
            {2, typeof (ContributionNotification)},
            {3, typeof (LoginRequest)},
        };

        public static readonly Dictionary<Type, int> MessageTypeRegistry = new Dictionary<Type, int>
        {
            {typeof (ContributionRequest), 1},
            {typeof (ContributionNotification), 2},
            {typeof (LoginRequest), 3}
        };
    }
}