using System.Collections.Generic;
using System.Text;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public class ConversationViewModel : ViewModel
    {
        private readonly Conversation conversation;
        private readonly ParticipationRepository participationRepository;
        private readonly UserRepository userRepository;

        public ConversationViewModel(Conversation conversation, UserRepository userRepository, ParticipationRepository participationRepository)
        {
            if (!IsInDesignMode)
            {
                this.conversation = conversation;
                this.userRepository = userRepository;
                this.participationRepository = participationRepository;
            }
        }

        public string ConversationParticipants
        {
            get { return GetConversationParticipants(); }
        }

        public int ConversationId
        {
            get { return conversation.Id; }
        }

        private string GetConversationParticipants()
        {
            var usernames = new List<string>();

            var titleBuilder = new StringBuilder();

            foreach (Participation participant in participationRepository.GetParticipationsByConversationId(conversation.Id))
            {
                usernames.Add(userRepository.FindEntityById(participant.UserId).Username);
            }

            titleBuilder.Append(TitleBuilder.CreateUserList(usernames));

            return titleBuilder.ToString();
        }
    }
}