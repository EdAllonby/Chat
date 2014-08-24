using SharedClasses;
using SharedClasses.Domain;

namespace Server
{
    /// <summary>
    /// Handles entities that have changed in the <see cref="EntityEntityRepository{T}"/>
    /// </summary>
    internal abstract class OnEntityChangedHandler
    {
        private readonly IClientManager clientManager;
        private readonly RepositoryManager repositoryManager;

        protected OnEntityChangedHandler(IServiceRegistry serviceRegistry)
        {
            repositoryManager = serviceRegistry.GetService<RepositoryManager>();
            clientManager = serviceRegistry.GetService<IClientManager>();
        }

        protected RepositoryManager RepositoryManager
        {
            get { return repositoryManager; }
        }

        protected IClientManager ClientManager
        {
            get { return clientManager; }
        }

        public abstract void StopOnMessageChangedHandling();
    }
}