using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ParticipationNotificationHandler : MessageHandler<EntityNotification<Participation>>
    {
        public ParticipationNotificationHandler(IServiceRegistry serviceRegistry) : base(serviceRegistry)
        {
        }

        protected override void HandleMessage(EntityNotification<Participation> message)
        {
            var participationRepository = (IEntityRepository<Participation>) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            participationRepository.AddEntity(message.Entity);
        }
    }
}