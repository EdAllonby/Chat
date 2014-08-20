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
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var clientDisconnection = (ClientDisconnection) message;

            UserRepository userRepository = serviceRegistry.GetService<RepositoryManager>().UserRepository;
            var clientManager = serviceRegistry.GetService<IClientManager>();

            clientManager.RemoveClientHandler(clientDisconnection.UserId);

            var connectionStatus = new ConnectionStatus(clientDisconnection.UserId, ConnectionStatus.Status.Disconnected);

            userRepository.UpdateUserConnectionStatus(connectionStatus);
        }
    }
}