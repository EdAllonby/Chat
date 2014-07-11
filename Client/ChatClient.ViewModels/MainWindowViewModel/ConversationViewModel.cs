using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public class ConversationViewModel : ViewModel
    {
        private readonly Conversation conversation;
        private readonly UserRepository userRepository;
        private readonly ParticipationRepository participationRepository;

        public ConversationViewModel(Conversation conversation, UserRepository userRepository, ParticipationRepository participationRepository)
        {
            this.conversation = conversation;
            this.userRepository = userRepository;
            this.participationRepository = participationRepository;
        }

        public string ConversationParticipants
        {
            get { return GetConversationParticipants(); }
        }

        public int ConversationId
        {
            get { return conversation.ConversationId; }
        }

        private string GetConversationParticipants()
        {
            List<string> usernames = new List<string>();

            StringBuilder titleBuilder = new StringBuilder();

            foreach (Participation participant in participationRepository.GetParticipationsByConversationId(conversation.ConversationId))
            {
                usernames.Add(userRepository.FindUserByID(participant.UserId).Username);
            }

            titleBuilder.Append(TitleBuilder.CreateUserList(usernames));

            return titleBuilder.ToString();
        }
    }
}
