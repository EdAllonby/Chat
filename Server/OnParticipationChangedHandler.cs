using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace Server
{
    internal sealed class OnParticipationChangedHandler : OnEntityChangedHandler
    {
        private readonly IReadOnlyEntityRepository<Conversation> conversationRepository;
        private readonly ParticipationRepository participationRepository;

        public OnParticipationChangedHandler(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            participationRepository = (ParticipationRepository) RepositoryManager.GetRepository<Participation>();
            conversationRepository = RepositoryManager.GetRepository<Conversation>();

            participationRepository.EntityAdded += OnParticipationAdded;
            participationRepository.EntityUpdated += OnParticipationChanged;
        }

        private void OnParticipationChanged(object sender, EntityChangedEventArgs<Participation> e)
        {
            if (!e.Entity.UserTyping.Equals(e.PreviousEntity.UserTyping))
            {
                OnUserTypingChanged(e.Entity);
            }
        }

        private void OnParticipationAdded(object sender, EntityChangedEventArgs<Participation> e)
        {
            Participation participation = e.Entity;

            List<Participation> conversationParticipants = participationRepository.GetParticipationsByConversationId(participation.ConversationId);

            var participationNotification = new EntityNotification<Participation>(participation, NotificationType.Create);

            IEnumerable<int> conversationParticipantUserIds = conversationParticipants.Select(conversationParticipation => conversationParticipation.UserId);

            ClientManager.SendMessageToClients(participationNotification, conversationParticipantUserIds);

            List<Participation> otherParticipants = conversationParticipants.Where(conversationParticipant => !conversationParticipant.Equals(participation)).ToList();

            otherParticipants.ForEach(
                otherParticipant => ClientManager.SendMessageToClient(new EntityNotification<Participation>(otherParticipant, NotificationType.Create), participation.UserId));

            Conversation conversation = conversationRepository.FindEntityById(participation.ConversationId);

            SendConversationNotificationToParticipants(conversation, participation.UserId, otherParticipants);
        }

        private void OnUserTypingChanged(Participation participation)
        {
            List<Participation> participationsForConversation = participationRepository.GetParticipationsByConversationId(participation.ConversationId);

            List<int> usersInConversation = participationsForConversation.Select(x => x.UserId).ToList();

            var userTypingNotification = new EntityNotification<UserTyping>(participation.UserTyping, NotificationType.Update);

            ClientManager.SendMessageToClients(userTypingNotification, usersInConversation);
        }

        private void SendConversationNotificationToParticipants(Conversation conversation, int newParticipantUserId, IEnumerable<Participation> otherParticipants)
        {
            if (conversation != null)
            {
                ClientManager.SendMessageToClient(new EntityNotification<Conversation>(conversation, NotificationType.Create), newParticipantUserId);

                IEnumerable<int> currentConversationParticipantUserIds = otherParticipants.Select(participant => participant.UserId);

                ClientManager.SendMessageToClients(new EntityNotification<Conversation>(conversation, NotificationType.Update), currentConversationParticipantUserIds);
            }
        }

        public override void StopOnMessageChangedHandling()
        {
            participationRepository.EntityAdded -= OnParticipationAdded;
        }
    }
}