using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="ParticipationNotification"/> the Client received.
    /// </summary>
    internal sealed class ParticipationNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IMessageContext context)
        {
            var participationNotification = (ParticipationNotification) message;
            var participationNotificationContext = (ParticipationNotificationContext) context;

            AddParticipationToRepository(participationNotification, participationNotificationContext);
        }

        private void AddParticipationToRepository(ParticipationNotification participationNotification,
            ParticipationNotificationContext participationNotificationContext)
        {
            participationNotificationContext.ParticipationRepository.AddEntity(participationNotification.Participation);
        }
    }
}