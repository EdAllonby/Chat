using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ChatClient.Services;
using ChatClient.ViewMediator;
using ChatClient.ViewModels.Commands;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.UserListViewModel
{
    /// <summary>
    /// Holds the logic for the view. Accesses the Service manager to receive and send messages. 
    /// </summary>
    public class UserListViewModel : ViewModel
    {
        private readonly IClientService clientService;
        private readonly RepositoryManager repositoryManager;

        private IList<ConnectedUserViewModel> connectedUsers = new List<ConnectedUserViewModel>();

        private bool isMultiUserConversation;

        public UserListViewModel()
        {
            clientService = ServiceManager.GetService<IClientService>();

            repositoryManager = clientService.RepositoryManager;

            GetAllUsers(repositoryManager.UserRepository.GetAllUsers());

            clientService.RepositoryManager.UserRepository.UserUpdated += OnUserUpdated;

            clientService.RepositoryManager.ConversationRepository.ConversationAdded += NewConversationNotification;

            clientService.RepositoryManager.ConversationRepository.ContributionAdded += NewContributionNotification;
        }

        public string Username
        {
            get { return repositoryManager.UserRepository.FindUserByID(clientService.ClientUserId).Username; }
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

        public static ICommand Closing
        {
            get { return new RelayCommand(() => Application.Current.Shutdown()); }
        }

        private static void NewConversationNotification(object sender, Conversation conversation)
        {
            CreateNewConversationWindow(conversation);
        }

        private void NewContributionNotification(object sender, Contribution contribution)
        {
            Conversation conversation =
                repositoryManager.ConversationRepository.FindConversationById(contribution.ConversationId);

            CreateNewConversationWindow(conversation);
        }

        public void StartNewSingleUserConversation(int participant)
        {
            var participantIds = new List<int> {clientService.ClientUserId, participant};

            NewConversation(participantIds);
        }

        private void StartNewMultiUserConversation()
        {
            var participantIds = new List<int> {clientService.ClientUserId};

            participantIds.AddRange(connectedUsers.Where(user => user.IsSelectedForConversation)
                .Select(connectedUser => connectedUser.UserId));

            NewConversation(participantIds);
        }

        private bool CanStartNewMultiUserConversation()
        {
            return connectedUsers.Any(connectedUser => connectedUser.IsSelectedForConversation);
        }

        private void OnUserUpdated(object sender, User user)
        {
            IEnumerable<User> users = repositoryManager.UserRepository.GetAllUsers();

            GetAllUsers(users);
        }

        private void GetAllUsers(IEnumerable<User> users)
        {
            List<User> newUserList = users.Where(user => user.UserId != clientService.ClientUserId)
                .Where(user => user.ConnectionStatus == ConnectionStatus.Connected).ToList();

            List<ConnectedUserViewModel> otherUsers = newUserList.Select(user => new ConnectedUserViewModel(user)).ToList();

            ConnectedUsers = otherUsers;
        }

        private void NewConversation(List<int> participantIds)
        {
            IsMultiUserConversation = false;

            if (!repositoryManager.ParticipationRepository.DoesConversationWithUsersExist(participantIds))
            {
                clientService.CreateConversation(participantIds);
            }
            else
            {
                int conversationId = repositoryManager.ParticipationRepository.GetConversationIdByParticipantsId(participantIds);
                CreateNewConversationWindow(repositoryManager.ConversationRepository.FindConversationById(conversationId));
            }
        }

        private static void CreateNewConversationWindow(Conversation conversation)
        {
            // Check if conversation window already exists
            if (ConversationWindowsStatusCollection.GetWindowStatus(conversation.ConversationId) == WindowStatus.Closed)
            {
                Application.Current.Dispatcher.Invoke(
                    () => Mediator.Instance.SendMessage(ViewName.ChatWindow, new ChatWindowViewModel.ChatWindowViewModel(conversation)));

                ConversationWindowsStatusCollection.SetWindowStatus(conversation.ConversationId, WindowStatus.Open);
            }
        }
    }
}