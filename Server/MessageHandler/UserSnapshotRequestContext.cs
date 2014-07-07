using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="UserSnapshotRequestHandler"/> needs. 
    /// </summary>
    internal sealed class UserSnapshotRequestContext : IMessageContext
    {
        private readonly Dictionary<int, ClientHandler> clientHandlersIndexedByUserId;
        private readonly UserRepository userRepository;

        public UserSnapshotRequestContext(UserRepository userRepository,
            Dictionary<int, ClientHandler> clientHandlersIndexedByUserId)
        {
            this.userRepository = userRepository;
            this.clientHandlersIndexedByUserId = clientHandlersIndexedByUserId;
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }

        public Dictionary<int, ClientHandler> ClientHandlersIndexedByUserId
        {
            get { return clientHandlersIndexedByUserId; }
        }
    }
}