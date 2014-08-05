using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ClientDisconnection"/> the Server received.
    /// </summary>
    internal sealed class ClientDisconnectionHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServerMessageContext context)
        {
            var clientDisconnection = (ClientDisconnection) message;

            context.ClientManager.RemoveClientHandler(clientDisconnection.UserId);

            var connectionStatus = new ConnectionStatus(clientDisconnection.UserId, ConnectionStatus.Status.Disconnected);

            context.RepositoryManager.UserRepository.UpdateUserConnectionStatus(connectionStatus);
        }
    }
}