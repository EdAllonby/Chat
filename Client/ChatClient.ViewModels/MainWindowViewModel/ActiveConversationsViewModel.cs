using System.Collections.Generic;
using System.Linq;
using ChatClient.Services;
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
            if (!IsInDesignModeStatic)
            {
                IClientService clientService = ServiceManager.GetService<IClientService>();

                repositoryManager = clientService.RepositoryManager;

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

        }


        private void UpdateActiveConversations()
        {
            IEnumerable<Conversation> conversations = repositoryManager.ConversationRepository.GetAllConversations();

            List<ConversationViewModel> updatedConversations = conversations.Select(conversation =>
                new ConversationViewModel(conversation, repositoryManager.UserRepository, repositoryManager.ParticipationRepository)).ToList();

            ActiveConversations = updatedConversations;
        }

        private void OnConversationUpdated(object sender, Conversation e)
        {
            UpdateActiveConversations();
        }

        private void OnConversationAdded(object sender, Conversation e)
        {
            UpdateActiveConversations();
        }
    }
}