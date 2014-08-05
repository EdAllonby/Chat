using SharedClasses;

namespace ChatClient.Services
{
    /// <summary>
    /// Represents the context of a client.
    /// </summary>
    internal interface IClientMessageContext
    {
        /// <summary>
        /// A client's collection of repositories.
        /// </summary>
        RepositoryManager RepositoryManager { get; }
    }
}