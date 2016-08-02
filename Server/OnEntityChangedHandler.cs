using SharedClasses;
using SharedClasses.Domain;

namespace Server
{
    /// <summary>
    /// Handles entities that have changed in the <see cref="EntityRepository{T}" />
    /// </summary>
    internal abstract class OnEntityChangedHandler
    {
        protected OnEntityChangedHandler(IServiceRegistry serviceRegistry)
        {
            RepositoryManager = serviceRegistry.GetService<RepositoryManager>();
            ClientManager = serviceRegistry.GetService<IClientManager>();
        }

        protected RepositoryManager RepositoryManager { get; }

        protected IClientManager ClientManager { get; }

        public abstract void StopOnMessageChangedHandling();
    }
}