using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="UserSnapshotRequest"/> the Server received.
    /// </summary>
    internal sealed class UserSnapshotRequestHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var userSnapshotRequest = (UserSnapshotRequest) message;
            var userSnapshotRequestContext = (UserSnapshotRequestContext) context;

            SendUserSnapshot(userSnapshotRequest, userSnapshotRequestContext);
        }

        private static void SendUserSnapshot(UserSnapshotRequest userSnapshotRequest,
            UserSnapshotRequestContext userSnapshotRequestContext)
        {
            IEnumerable<User> currentUsers = userSnapshotRequestContext.UserRepository.GetAllUsers();
            var userSnapshot = new UserSnapshot(currentUsers);
            userSnapshotRequestContext.ClientHandlersIndexedByUserId[userSnapshotRequest.UserId].SendMessage(userSnapshot);
        }
    }
}