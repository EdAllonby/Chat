using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    public sealed class UserTypingRequestHandler : IMessageHandler
    {
        /// <summary>
        /// When a <see cref="UserTypingRequest"/> message is sent to the server, the server will notify all users in the conversation of the user typing.
        /// </summary>
        /// <param name="message">The <see cref="UserTypingRequest"/> message.</param>
        /// <param name="serviceRegistry">Contains all services for the server.</param>
        public void HandleMessage(IMessage message, IServiceRegistry serviceRegistry)
        {
            var userTypingRequest = (UserTypingRequest) message;

            var participationRepository = (IEntityRepository<Participation>) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

            Participation participation = participationRepository.FindEntityById(userTypingRequest.UserTyping.ParticipationId);

            Participation clonedParticipation = Participation.DeepClone(participation);

            clonedParticipation.UserTyping = userTypingRequest.UserTyping;

            participationRepository.UpdateEntity(clonedParticipation);
        }
    }
}