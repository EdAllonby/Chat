using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ClientDisconnection" /> the Server received.
    /// </summary>
    internal sealed class ClientDisconnectionHandler : MessageHandler<ClientDisconnection>
    {
        public ClientDisconnectionHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(ClientDisconnection message)
        {
            var userRepository = (UserRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();

            var clientManager = ServiceRegistry.GetService<IClientManager>();

            clientManager.RemoveClientHandler(message.UserId);

            var connectionStatus = new ConnectionStatus(message.UserId, ConnectionStatus.Status.Disconnected);

            userRepository.UpdateUserConnectionStatus(connectionStatus);

            SendUserTypingNotification(message.UserId, clientManager, ServiceRegistry.GetService<RepositoryManager>());
        }

        /// <summary>
        /// If a user unexpectedly disconnects whilst sending a message, clients will be unaware and continue seeing the user as
        /// typing a message.
        /// This procedure will go through conversations a client is in and forcefully tell other clients that the user has stopped
        /// typing.
        /// </summary>
        /// <remarks>
        /// This could have be done in the Client when a client senses that a user's <see cref="ConnectionStatus" /> has been
        /// modified.
        /// But I'd rather have it as a rule here where a client can use their same
        /// <see cref="EntityNotification{UserNotification}" /> logic.
        /// </remarks>
        /// <param name="userId">The user that disconnected.</param>
        /// <param name="clientManager">Holds the connected clients.</param>
        /// <param name="repositoryManager">Holds the repositories in the system.</param>
        private static void SendUserTypingNotification(int userId, IClientManager clientManager, RepositoryManager repositoryManager)
        {
            var participationRepository = (ParticipationRepository) repositoryManager.GetRepository<Participation>();
            var userRepository = (UserRepository) repositoryManager.GetRepository<User>();

            IEnumerable<int> conversationIdsUserIsIn = participationRepository.GetAllConversationIdsByUserId(userId);

            foreach (int conversationId in conversationIdsUserIsIn)
            {
                Participation participation = participationRepository.GetParticipationByUserIdandConversationId(userId, conversationId);
                var userTyping = new UserTyping(false, participation.Id);
                var userTypingNotification = new EntityNotification<UserTyping>(userTyping, NotificationType.Create);
                List<Participation> participationsForConversation = participationRepository.GetParticipationsByConversationId(conversationId);

                List<int> userIdsInConversation = participationsForConversation.Select(x => x.UserId).ToList();

                foreach (int userIdInConversation in userIdsInConversation)
                {
                    User user = userRepository.FindEntityById(userIdInConversation);

                    if (user.ConnectionStatus.UserConnectionStatus != ConnectionStatus.Status.Disconnected)
                    {
                        clientManager.SendMessageToClient(userTypingNotification, userIdInConversation);
                    }
                }
            }
        }
    }
}