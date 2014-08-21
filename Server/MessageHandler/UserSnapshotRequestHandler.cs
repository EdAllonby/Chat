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
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var userSnapshotRequest = (UserSnapshotRequest) message;

            IReadOnlyRepository<User> userRepository = serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();
            var clientManager = serviceRegistry.GetService<IClientManager>();

            IEnumerable<User> currentUsers = userRepository.GetAllEntities();
            var userSnapshot = new UserSnapshot(currentUsers);

            clientManager.SendMessageToClient(userSnapshot, userSnapshotRequest.UserId);
        }
    }
}