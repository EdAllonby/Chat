using System;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntitySnapshot{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class UserSnapshotHandler : MessageHandler<EntitySnapshot<User>>, IBootstrapper
    {
        public UserSnapshotHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntitySnapshot<User> message)
        {
            var userRepository = (UserRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            foreach (User user in message.Entities)
            {
                userRepository.AddEntity(user);
            }

            OnUserBootstrapCompleted();
        }

        private void OnUserBootstrapCompleted()
        {
            var handler = EntityBootstrapCompleted;

            if (handler != null)
            {
                handler(this, new EntityBootstrapEventArgs(typeof(User)));
            }
        }

        public event EventHandler<EntityBootstrapEventArgs> EntityBootstrapCompleted;
    }
}