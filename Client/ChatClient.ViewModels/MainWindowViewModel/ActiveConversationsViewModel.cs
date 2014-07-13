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

                repositoryManager.ConversationRepository.ConversationAdded += OnConversationAdded;
                repositoryManager.ConversationRepository.ConversationUpdated += OnConversationUpdated;
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

        public void GetConversationWindow(int conversationId)
        {
            WindowManager.CreateConversationWindow(repositoryManager.ConversationRepository.FindConversationById(conversationId));
        }

        private void UpdateActiveConversations()
        {
            IEnumerable<Conversation> conversations = repositoryManager.ConversationRepository.GetAllConversations();

            List<ConversationViewModel> updatedConversations = conversations.Select(conversation =>
                new ConversationViewModel(conversation, repositoryManager.UserRepository, repositoryManager.ParticipationRepository)).ToList();

            ActiveConversations = updatedConversations;
        }

        private void OnConversationUpdated(object sender, Conversation conversation)
        {
            UpdateActiveConversations();
        }

        private void OnConversationAdded(object sender, Conversation e)
        {
            UpdateActiveConversations();
        }
    }
}