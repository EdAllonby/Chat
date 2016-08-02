using System.Collections.Generic;
using System.Linq;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class ActiveConversationsViewModel : ViewModel
    {
        private readonly IReadOnlyEntityRepository<Conversation> conversationRepository;

        private IList<ConversationViewModel> activeConversations = new List<ConversationViewModel>();

        public ActiveConversationsViewModel(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                conversationRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();

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
            ConversationWindowManager.CreateConversationWindow(ServiceRegistry, conversationRepository.FindEntityById(conversationId));
        }

        private void UpdateActiveConversations()
        {
            IEnumerable<Conversation> conversations = conversationRepository.GetAllEntities();

            List<ConversationViewModel> updatedConversations = conversations.Select(conversation =>
                new ConversationViewModel(conversation, ServiceRegistry)).ToList();

            ActiveConversations = updatedConversations;
        }
    }
}