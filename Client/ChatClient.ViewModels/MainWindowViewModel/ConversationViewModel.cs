using System.Collections.Generic;
using System.Text;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public class ConversationViewModel : ViewModel
    {
        private readonly Conversation conversation;
        private readonly ParticipationRepository participationRepository;
        private readonly IReadOnlyEntityRepository<User> userRepository;

        public ConversationViewModel(Conversation conversation, IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                this.conversation = conversation;
                var repositoryManager = serviceRegistry.GetService<RepositoryManager>();
                userRepository = repositoryManager.GetRepository<User>();
                participationRepository = (ParticipationRepository) repositoryManager.GetRepository<Participation>();
            }
        }

        public string ConversationParticipants => GetConversationParticipants();

        public int ConversationId => conversation.Id;

        private string GetConversationParticipants()
        {
            var usernames = new List<string>();

            var titleBuilder = new StringBuilder();

            foreach (Participation participant in participationRepository.GetParticipationsByConversationId(conversation.Id))
            {
                usernames.Add(userRepository.FindEntityById(participant.UserId).Username);
            }

            titleBuilder.Append(ChatWindowStringBuilder.CreateUserListTitle(usernames));

            return titleBuilder.ToString();
        }
    }
}