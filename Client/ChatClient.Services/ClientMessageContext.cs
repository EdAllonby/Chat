using SharedClasses;

namespace ChatClient.Services
{
    /// <summary>
    /// Represents the context of a client.
    /// </summary>
    internal class ClientMessageContext : IClientMessageContext
    {
        public ClientMessageContext(RepositoryManager repositoryManager)
        {
            RepositoryManager = repositoryManager;
        }

        /// <summary>
        /// A client's collection of repositories.
        /// </summary>
        public RepositoryManager RepositoryManager { get; private set; }
    }
}