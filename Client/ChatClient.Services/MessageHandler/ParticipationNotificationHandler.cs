using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationNotification"/> the Client received.
    /// </summary>
    internal sealed class ParticipationNotificationHandler : IClientMessageHandler
    {
        public void HandleMessage(IMessage message, IClientMessageContext context)
        {
            var participationNotification = (ParticipationNotification) message;

            context.RepositoryManager.ParticipationRepository.AddEntity(participationNotification.Participation);
        }
    }
}