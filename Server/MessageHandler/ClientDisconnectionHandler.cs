using System.Collections.Generic;
using log4net;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ClientDisconnection"/> the Server received.
    /// </summary>
    internal sealed class ClientDisconnectionHandler : IMessageHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ClientDisconnectionHandler));

        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var clientDisconnection = (ClientDisconnection) message;
            var clientDisconnectionContext = (ClientDisconnectionContext) context;

            RemoveClientHandler(clientDisconnection.UserId, clientDisconnectionContext.ClientHandlersIndexedByUserId);
            DisconnectUser(clientDisconnection.UserId, clientDisconnectionContext.UserRepository);
        }

        private static void RemoveClientHandler(int disconnectedUserId,
            IDictionary<int, ClientHandler> clientHandlersIndexedByUserId)
        {
            clientHandlersIndexedByUserId.Remove(disconnectedUserId);
            Log.Info("User with id " + disconnectedUserId + " logged out. Removing from ServerHandler's ConnectionHandler list");
        }

        private static void DisconnectUser(int disconnectedUserId, UserRepository userRepository)
        {
            User user = userRepository.FindUserByID(disconnectedUserId);
            user.ConnectionStatus = ConnectionStatus.Disconnected;
            userRepository.UpdateUser(user);
        }
    }
}