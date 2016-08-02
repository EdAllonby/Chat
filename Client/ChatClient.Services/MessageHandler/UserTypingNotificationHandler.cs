using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    public sealed class UserTypingNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var userTypingNotification = (EntityNotification<UserTyping>) message;

            var participationRepository = (IEntityRepository<Participation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            Participation participation = participationRepository.FindEntityById(userTypingNotification.Entity.ParticipationId);

            Participation clonedParticipation = Participation.DeepClone(participation);

            clonedParticipation.UserTyping = userTypingNotification.Entity;

            participationRepository.UpdateEntity(clonedParticipation);
        }
    }
}