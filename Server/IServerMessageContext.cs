using SharedClasses;

namespace Server
{
    /// <summary>
    /// Represents the context of a server.
    /// </summary>
    internal interface IServerMessageContext
    {
        /// <summary>
        /// A server's collection of repositories.
        /// </summary>
        RepositoryManager RepositoryManager { get; }

        /// <summary>
        /// A server's Id allocator.
        /// </summary>
        EntityIdAllocatorFactory EntityIdAllocatorFactory { get; }

        /// <summary>
        /// A server's collection of <see cref="ClientHandler"/>s.
        /// </summary>
        IClientManager ClientManager { get; }
    }
}