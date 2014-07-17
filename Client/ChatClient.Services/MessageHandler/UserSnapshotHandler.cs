using System;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="UserSnapshot"/> the Client received.
    /// </summary>
    internal sealed class UserSnapshotHandler : IMessageHandler
    {
        public event EventHandler UserBootstrapCompleted;

        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var userSnapshot = (UserSnapshot) message;
            var userSnapshotContext = (UserSnapshotContext) context;

            userSnapshotContext.UserRepository.AddUsers(userSnapshot.Users);
            
            OnUserBootstrapCompleted();
        }

        private void OnUserBootstrapCompleted()
        {
            EventHandler handler = UserBootstrapCompleted;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}