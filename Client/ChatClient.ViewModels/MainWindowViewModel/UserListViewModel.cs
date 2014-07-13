using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ChatClient.ViewModels.Commands;
using SharedClasses;
using SharedClasses.Domain;

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

                ClientService.RepositoryManager.UserRepository.UserAdded += OnUserAdded;
                ClientService.RepositoryManager.UserRepository.UserUpdated += OnUserUpdated;
                ClientService.RepositoryManager.ConversationRepository.ConversationAdded += OnConversationAdded;
                ClientService.RepositoryManager.ConversationRepository.ContributionAdded += OnContributionAdded;

                UpdateConnectedUsers();
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

        private static void OnConversationAdded(object sender, Conversation conversation)
        {
            WindowManager.CreateConversationWindow(conversation);
        }

        private void OnContributionAdded(object sender, Contribution contribution)
        {
            Conversation conversation =
                repositoryManager.ConversationRepository.FindConversationById(contribution.ConversationId);

            WindowManager.CreateConversationWindow(conversation);
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

        private void OnUserAdded(object sender, User user)
        {
            UpdateConnectedUsers();
        }

        private void OnUserUpdated(object sender, User user)
        {
            UpdateConnectedUsers();
        }

        private void UpdateConnectedUsers()
        {
            IEnumerable<User> users = repositoryManager.UserRepository.GetAllUsers();
            List<User> newUserList = users.Where(user => user.UserId != ClientService.ClientUserId)
                .Where(user => user.ConnectionStatus == ConnectionStatus.Connected).ToList();

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
                WindowManager.CreateConversationWindow(repositoryManager.ConversationRepository.FindConversationById(conversationId));
            }
        }
    }
}