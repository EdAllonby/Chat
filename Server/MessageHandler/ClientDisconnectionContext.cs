using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;

namespace Server.MessageHandler
{
    /// <summary>
    /// Holds a reference to the necessary objects a <see cref="ClientDisconnectionHandler"/> needs. 
    /// </summary>
    internal sealed class ClientDisconnectionContext : IMessageContext
    {
        private readonly Dictionary<int, ClientHandler> clientHandlersIndexedByUserId;
        private readonly UserRepository userRepository;

        public ClientDisconnectionContext(Dictionary<int, ClientHandler> clientHandlersIndexedByUserId,
            UserRepository userRepository)
        {
            this.clientHandlersIndexedByUserId = clientHandlersIndexedByUserId;
            this.userRepository = userRepository;
        }

        public Dictionary<int, ClientHandler> ClientHandlersIndexedByUserId
        {
            get { return clientHandlersIndexedByUserId; }
        }

        public UserRepository UserRepository
        {
            get { return userRepository; }
        }
    }
}