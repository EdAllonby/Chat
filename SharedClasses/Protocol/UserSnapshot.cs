using System;
using System.Collections.Generic;
using SharedClasses.Domain;

namespace SharedClasses.Protocol
{
    /// <summary>
    /// Sends a list of currently connected users to the recently logged in Client
    /// </summary>
    [Serializable]
    public class UserSnapshot : IMessage
    {
        public UserSnapshot(IList<User> users)
        {
            Users = users;
            Identifier = SerialiserRegistry.IdentifiersByMessageType[typeof (UserSnapshot)];
        }

        public IList<User> Users { get; private set; }
        public int Identifier { get; private set; }
    }
}