using System.Collections.Generic;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntitySnapshotRequest{User}" /> the Server received.
    /// </summary>
    internal sealed class UserSnapshotRequestHandler : MessageHandler<EntitySnapshotRequest<User>>
    {
        public UserSnapshotRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntitySnapshotRequest<User> message)
        {

            IReadOnlyEntityRepository<User> userRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();
            var clientManager = ServiceRegistry.GetService<IClientManager>();

            IEnumerable<User> currentUsers = userRepository.GetAllEntities();
            var userSnapshot = new EntitySnapshot<User>(currentUsers);

            clientManager.SendMessageToClient(userSnapshot, message.UserId);
        }
    }
}