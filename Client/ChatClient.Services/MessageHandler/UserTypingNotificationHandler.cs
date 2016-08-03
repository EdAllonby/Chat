using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    public sealed class UserTypingNotificationHandler : MessageHandler<EntityNotification<UserTyping>>
    {
        public UserTypingNotificationHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntityNotification<UserTyping> message)
        {
            var participationRepository = (IEntityRepository<Participation>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            Participation participation = participationRepository.FindEntityById(message.Entity.ParticipationId);

            Participation clonedParticipation = Participation.DeepClone(participation);

            clonedParticipation.UserTyping = message.Entity;

            participationRepository.UpdateEntity(clonedParticipation);
        }
    }
}