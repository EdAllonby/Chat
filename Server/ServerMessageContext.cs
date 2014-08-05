using SharedClasses;

namespace Server
{
    /// <summary>
    /// Represents the context of a server.
    /// </summary>
    internal class ServerMessageContext : IServerMessageContext
    {
        public ServerMessageContext(IClientManager clientManager, EntityIdAllocatorFactory entityIdAllocatorFactory, RepositoryManager repositoryManager)
        {
            RepositoryManager = repositoryManager;
            EntityIdAllocatorFactory = entityIdAllocatorFactory;
            ClientManager = clientManager;
        }

        /// <summary>
        /// A server's collection of repositories.
        /// </summary>
        public RepositoryManager RepositoryManager { get; private set; }

        /// <summary>
        /// A server's Id allocator.
        /// </summary>
        public EntityIdAllocatorFactory EntityIdAllocatorFactory { get; private set; }


        /// <summary>
        /// A server's collection of <see cref="ClientHandler"/>s.
        /// </summary>
        public IClientManager ClientManager { get; private set; }
    }
}