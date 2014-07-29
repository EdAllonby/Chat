using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ChatClient.ViewModels.Commands;
using SharedClasses;
using SharedClasses.Domain;
using SharedClasses.Message;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    /// <summary>
    /// Holds the logic for the view. Accesses the Service manager to receive and send messages. 
    /// </summary>
    public class UserListViewModel : ViewModel
    {
        private readonly RepositoryManager repositoryManager;

        private IList<ConnectedUserViewModel> connectedUsers = new List<ConnectedUserViewModel>();

        private bool isMultiUserConversation;

        public UserListViewModel()
        {
            if (!IsInDesignMode)
            {
                repositoryManager = ClientService.RepositoryManager;

                repositoryManager.UserRepository.EntityChanged += OnUserChanged;

                repositoryManager.ConversationRepository.EntityChanged += OnConversationChanged;

                UpdateConnectedUsers();
            }
        }

        private void OnConversationChanged(object sender, EntityChangedEventArgs<Conversation> e)
        {
            switch (e.NotificationType)
            {
                case NotificationType.Create:
                    OnConversationAdded(e.Entity);
                    break;
                case NotificationType.Update:
                    if (!e.Entity.LastContribution.Equals(e.PreviousEntity.LastContribution))
                    {
                        OnContributionAdded(e.Entity.LastContribution);
                    }
                    break;
            }
        }

        public bool IsMultiUserConversation
        {
            get { return isMultiUserConversation; }
            set
            {
                foreach (ConnectedUserViewModel connectedUser in ConnectedUsers)
                {
                    connectedUser.MultiUserSelectionMode = value;
                }

                if (Equals(value, isMultiUserConversation))
                {
                    return;
                }

                isMultiUserConversation = value;
                OnPropertyChanged();
            }
        }

        public IList<ConnectedUserViewModel> ConnectedUsers
        {
            get { return connectedUsers; }
            set
            {
                if (Equals(value, connectedUsers))
                {
                    return;
                }

                connectedUsers = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartMultiUserConversation
        {
            get { return new RelayCommand(StartNewMultiUserConversation, CanStartNewMultiUserConversation); }
        }

        private static void OnConversationAdded(Conversation conversation)
        {
            ConversationWindowManager.CreateConversationWindow(conversation);
        }

        private void OnContributionAdded(Contribution contribution)
        {
            Conversation conversation = repositoryManager.ConversationRepository.FindEntityById(contribution.ConversationId);

            ConversationWindowManager.CreateConversationWindow(conversation);
        }

        public void StartNewSingleUserConversation(int participant)
        {
            var participantIds = new List<int> {ClientService.ClientUserId, participant};

            NewConversation(participantIds);
        }

        private void StartNewMultiUserConversation()
        {
            var participantIds = new List<int> {ClientService.ClientUserId};

            participantIds.AddRange(connectedUsers.Where(user => user.IsSelectedForConversation)
                .Select(connectedUser => connectedUser.UserId));

            NewConversation(participantIds);
        }

        private bool CanStartNewMultiUserConversation()
        {
            return connectedUsers.Any(connectedUser => connectedUser.IsSelectedForConversation);
        }

        void OnUserChanged(object sender, EntityChangedEventArgs<User> e)
        {
            UpdateConnectedUsers();
        }


        private void UpdateConnectedUsers()
        {
            IEnumerable<User> users = repositoryManager.UserRepository.GetAllEntities();
            List<User> newUserList = users.Where(user => user.Id != ClientService.ClientUserId).ToList();

            List<ConnectedUserViewModel> otherUsers = newUserList.Select(user => new ConnectedUserViewModel(user)).ToList();

            ConnectedUsers = otherUsers;
        }

        private void NewConversation(List<int> participantIds)
        {
            IsMultiUserConversation = false;

            if (!repositoryManager.ParticipationRepository.DoesConversationWithUsersExist(participantIds))
            {
                ClientService.CreateConversation(participantIds);
            }
            else
            {
                int conversationId = repositoryManager.ParticipationRepository.GetConversationIdByParticipantsId(participantIds);
                ConversationWindowManager.CreateConversationWindow(repositoryManager.ConversationRepository.FindEntityById(conversationId));
            }
        }
    }
}