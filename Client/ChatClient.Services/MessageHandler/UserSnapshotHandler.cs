using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="UserSnapshot"/> the Client received.
    /// </summary>
    internal sealed class UserSnapshotHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            UserSnapshot userSnapshot = (UserSnapshot) message;
            UserSnapshotContext userSnapshotContext = (UserSnapshotContext) context;

            userSnapshotContext.UserRepository.AddUsers(userSnapshot.Users);
        }
    }
}