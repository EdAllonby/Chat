using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class ActiveConversationsViewModel : ViewModel
    {
        private readonly IReadOnlyRepository<User> userRepository;
        private readonly IReadOnlyRepository<Conversation> conversationRepository;
        private readonly ParticipationRepository participationRepository; 

        private IList<ConversationViewModel> activeConversations = new List<ConversationViewModel>();

        public ActiveConversationsViewModel()
        {
            if (!IsInDesignMode)
            {
                userRepository = ServiceManager.GetService<RepositoryManager>().GetRepository<User>();
                conversationRepository = ServiceManager.GetService<RepositoryManager>().GetRepository<Conversation>();
                participationRepository = (ParticipationRepository) ServiceManager.GetService<RepositoryManager>().GetRepository<Participation>();

                UpdateActiveConversations();

                conversationRepository.EntityAdded += OnConversationChanged;
                conversationRepository.EntityUpdated += OnConversationChanged;
            }
        }

        public IList<ConversationViewModel> ActiveConversations
        {
            get { return activeConversations; }
            set
            {
                if (Equals(value, activeConversations))
                {
                    return;
                }

                activeConversations = value;
                OnPropertyChanged();
            }
        }

        private void OnConversationChanged(object sender, EntityChangedEventArgs<Conversation> e)
        {
            UpdateActiveConversations();
        }

        public void GetConversationWindow(int conversationId)
        {
            ConversationWindowManager.CreateConversationWindow(conversationRepository.FindEntityById(conversationId));
        }

        private void UpdateActiveConversations()
        {
            IEnumerable<Conversation> conversations = conversationRepository.GetAllEntities();

            List<ConversationViewModel> updatedConversations = conversations.Select(conversation =>
                new ConversationViewModel(conversation, userRepository, participationRepository)).ToList();

            ActiveConversations = updatedConversations;
        }
    }
}