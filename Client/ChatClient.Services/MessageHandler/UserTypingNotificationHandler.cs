using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    public sealed class UserTypingNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var userTypingNotification = (UserTypingNotification) message;

            var participationRepository = (IEntityRepository<Participation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            Participation participation = participationRepository.FindEntityById(userTypingNotification.UserTyping.ParticipationId);

            Participation clonedParticipation = Participation.DeepClone(participation);

            clonedParticipation.UserTyping = userTypingNotification.UserTyping;

            participationRepository.UpdateEntity(clonedParticipation);
        }
    }
}