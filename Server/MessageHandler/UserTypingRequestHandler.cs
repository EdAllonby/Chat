using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    public sealed class UserTypingRequestHandler : MessageHandler<UserTypingRequest>
    {
        public UserTypingRequestHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(UserTypingRequest message)
        {
            var participationRepository = (IEntityRepository<Participation>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            Participation participation = participationRepository.FindEntityById(message.UserTyping.ParticipationId);

            Participation clonedParticipation = Participation.DeepClone(participation);

            clonedParticipation.UserTyping = message.UserTyping;

            participationRepository.UpdateEntity(clonedParticipation);
        }
    }
}