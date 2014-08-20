using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class ActiveConversationsViewModel : ViewModel
    {
        private readonly RepositoryManager repositoryManager;

        private IList<ConversationViewModel> activeConversations = new List<ConversationViewModel>();

        public ActiveConversationsViewModel()
        {
            if (!IsInDesignMode)
            {
                repositoryManager = ClientService.RepositoryManager;

                UpdateActiveConversations();

                repositoryManager.ConversationRepository.EntityAdded += OnConversationChanged;
                repositoryManager.ConversationRepository.EntityUpdated += OnConversationChanged;
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
            ConversationWindowManager.CreateConversationWindow(repositoryManager.ConversationRepository.FindEntityById(conversationId));
        }

        private void UpdateActiveConversations()
        {
            IEnumerable<Conversation> conversations = repositoryManager.ConversationRepository.GetAllEntities();

            List<ConversationViewModel> updatedConversations = conversations.Select(conversation =>
                new ConversationViewModel(conversation, repositoryManager.UserRepository, repositoryManager.ParticipationRepository)).ToList();

            ActiveConversations = updatedConversations;
        }
    }
}