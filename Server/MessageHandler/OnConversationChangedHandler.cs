using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    internal sealed class OnConversationChangedHandler : OnEntityChangedHandler
    {
        public OnConversationChangedHandler(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            RepositoryManager.ConversationRepository.EntityAdded += OnConversationAdded;
            RepositoryManager.ConversationRepository.EntityUpdated += OnConversationUpdated;
        }

        private void OnConversationAdded(object sender, EntityChangedEventArgs<Conversation> e)
        {
            OnConversationAdded(e.Entity);
        }

        private void OnConversationAdded(Conversation conversation)
        {
            var conversationNotification = new ConversationNotification(conversation, NotificationType.Create);

            IEnumerable<int> userIds =
                RepositoryManager.ParticipationRepository.GetParticipationsByConversationId(conversation.Id)
                    .Select(participation => participation.UserId);

            ClientManager.SendMessageToClients(conversationNotification, userIds);
        }

        private void OnConversationUpdated(object sender, EntityChangedEventArgs<Conversation> e)
        {
            if (!e.Entity.LastContribution.Equals(e.PreviousEntity.LastContribution))
            {
                OnContributionAdded(e.Entity.LastContribution);
            }
        }

        private void OnContributionAdded(Contribution contribution)
        {
            var contributionNotification = new ContributionNotification(contribution, NotificationType.Create);

            List<Participation> participationsByConversationId =
                RepositoryManager.ParticipationRepository.GetParticipationsByConversationId(contribution.ConversationId);
            IEnumerable<User> participantUsers =
                participationsByConversationId.Select(
                    participant => RepositoryManager.UserRepository.FindEntityById(participant.UserId));
            IEnumerable<int> connectedUserIds =
                participantUsers.Where(user => user.ConnectionStatus.UserConnectionStatus == ConnectionStatus.Status.Connected)
                    .Select(user => user.Id);

            ClientManager.SendMessageToClients(contributionNotification, connectedUserIds);
        }

        public override void StopOnMessageChangedHandling()
        {
            RepositoryManager.ConversationRepository.EntityAdded -= OnConversationAdded;
            RepositoryManager.ConversationRepository.EntityUpdated -= OnConversationUpdated;
        }
    }
}