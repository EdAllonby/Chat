using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.Services.MessageHandler
{
    /// <summary>
    /// Handles a <see cref="EntityNotification{TEntity}" /> the Client received.
    /// </summary>
    internal sealed class ParticipationNotificationHandler : IMessageHandler
    {
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var participationNotification = (EntityNotification<Participation>) message;

            var participationRepository = (IEntityRepository<Participation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            participationRepository.AddEntity(participationNotification.Entity);
        }
    }
}