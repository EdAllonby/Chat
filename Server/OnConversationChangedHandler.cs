using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server
{
    internal sealed class OnConversationChangedHandler : OnEntityChangedHandler
    {
        private readonly IReadOnlyEntityRepository<Conversation> conversationRepository;
        private readonly ParticipationRepository participationRepository;
        private readonly IReadOnlyEntityRepository<User> userRepository;

        public OnConversationChangedHandler(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            conversationRepository = RepositoryManager.GetRepository<Conversation>();
            participationRepository = (ParticipationRepository) RepositoryManager.GetRepository<Participation>();
            userRepository = RepositoryManager.GetRepository<User>();

            conversationRepository.EntityAdded += OnConversationAdded;
            conversationRepository.EntityUpdated += OnConversationUpdated;
        }

        private void OnConversationAdded(object sender, EntityChangedEventArgs<Conversation> e)
        {
            var conversationNotification = new EntityNotification<Conversation>(e.Entity, NotificationType.Create);

            IEnumerable<int> userIds = participationRepository.GetParticipationsByConversationId(e.Entity.Id).Select(participation => participation.UserId);

            ClientManager.SendMessageToClients(conversationNotification, userIds);
        }

        private void OnConversationUpdated(object sender, EntityChangedEventArgs<Conversation> e)
        {
            if (!e.Entity.LastContribution.Equals(e.PreviousEntity.LastContribution))
            {
                OnContributionAdded(e.Entity.LastContribution);
            }
        }

        private void OnContributionAdded(IContribution contribution)
        {
            var contributionNotification = new EntityNotification<IContribution>(contribution, NotificationType.Create);

            List<Participation> participationsByConversationId = participationRepository.GetParticipationsByConversationId(contribution.ConversationId);

            IEnumerable<User> participantUsers = participationsByConversationId.Select(participant => userRepository.FindEntityById(participant.UserId));

            IEnumerable<int> connectedUserIds = participantUsers.Where(user => user.ConnectionStatus.UserConnectionStatus == ConnectionStatus.Status.Connected).Select(user => user.Id);

            ClientManager.SendMessageToClients(contributionNotification, connectedUserIds);
        }

        public override void StopOnMessageChangedHandling()
        {
            conversationRepository.EntityAdded -= OnConversationAdded;
            conversationRepository.EntityUpdated -= OnConversationUpdated;
        }
    }
}