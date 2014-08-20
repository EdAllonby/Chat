using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server.MessageHandler
{
    internal sealed class OnParticipationChangedHandler : OnEntityChangedHandler
    {
        public OnParticipationChangedHandler(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            RepositoryManager.ParticipationRepository.EntityAdded += OnParticipationAdded;
        }

        private void OnParticipationAdded(object sender, EntityChangedEventArgs<Participation> e)
        {
            Participation participation = e.Entity;

            List<Participation> conversationParticipants = RepositoryManager.ParticipationRepository.GetParticipationsByConversationId(participation.ConversationId);

            var participationNotification = new ParticipationNotification(participation, NotificationType.Create);

            IEnumerable<int> conversationParticipantUserIds = conversationParticipants.Select(conversationParticipation => conversationParticipation.UserId);

            ClientManager.SendMessageToClients(participationNotification, conversationParticipantUserIds);

            List<Participation> otherParticipants = conversationParticipants.Where(conversationParticipant => !conversationParticipant.Equals(participation)).ToList();

            otherParticipants.ForEach(
                otherParticipant => ClientManager.SendMessageToClient(new ParticipationNotification(otherParticipant, NotificationType.Create), participation.UserId));

            Conversation conversation = RepositoryManager.ConversationRepository.FindEntityById(participation.ConversationId);

            SendConversationNotificationToParticipants(conversation, participation.UserId, otherParticipants);
        }

        private void SendConversationNotificationToParticipants(Conversation conversation, int newParticipantUserId, IEnumerable<Participation> otherParticipants)
        {
            if (conversation != null)
            {
                ClientManager.SendMessageToClient(new ConversationNotification(conversation, NotificationType.Create), newParticipantUserId);

                IEnumerable<int> currentConversationParticipantUserIds = otherParticipants.Select(participant => participant.UserId);

                ClientManager.SendMessageToClients(new ConversationNotification(conversation, NotificationType.Update), currentConversationParticipantUserIds);
            }
        }

        public override void StopOnMessageChangedHandling()
        {
            RepositoryManager.ParticipationRepository.EntityAdded -= OnParticipationAdded;
        }
    }
}