using System.Collections.Generic;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="UserSnapshotRequest"/> the Server received.
    /// </summary>
    internal sealed class UserSnapshotRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var userSnapshotRequest = (UserSnapshotRequest) message;

            IEnumerable<User> currentUsers = context.RepositoryManager.UserRepository.GetAllEntities();
            var userSnapshot = new UserSnapshot(currentUsers);

            context.ClientManager.SendMessageToClient(userSnapshot, userSnapshotRequest.UserId);
        }
    }
}