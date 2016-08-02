using System;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="UserSnapshot"/> the Client received.
    /// </summary>
    internal sealed class UserSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var userSnapshot = (UserSnapshot) message;

            var userRepository = (UserRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            foreach (User user in userSnapshot.Users)
            {
                userRepository.AddEntity(user);
            }

            OnUserBootstrapCompleted();
        }

        public event EventHandler UserBootstrapCompleted;

        private void OnUserBootstrapCompleted()
        {
            EventHandler handler = UserBootstrapCompleted;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}