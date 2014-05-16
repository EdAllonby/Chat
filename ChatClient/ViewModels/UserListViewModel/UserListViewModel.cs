using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using ChatClient.Views;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    internal class UserListViewModel : ViewModel
    {
        private IList<ConnectedUserViewModel> connectedUsers = new List<ConnectedUserViewModel>();
        private bool isMultiUserConversation;

        public UserListViewModel()
        {
            Client.OnNewUser += OnNewUser;

            Client.OnNewConversationNotification += OnNewConversationNotification;

            Client.OnNewContributionNotification += OnNewContributionNotification;

            Username = Client.Username;
        }

        public string Username { get; private set; }

        public bool IsMultiUserConversation
        {
            get { return isMultiUserConversation; }
            set
            {
                foreach (ConnectedUserViewModel connectedUser in ConnectedUsers)
                {
                    connectedUser.MultiUserSelectionMode = value;
                }

                if (Equals(value, isMultiUserConversation)) return;
                isMultiUserConversation = value;
                OnPropertyChanged();
            }
        }

        public IList<ConnectedUserViewModel> ConnectedUsers
        {
            get { return connectedUsers; }
            set
            {
                if (Equals(value, connectedUsers)) return;
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

        private static void OnNewConversationNotification(Conversation conversation)
        {
            CreateNewConversationWindow(conversation);
        }

        private static void OnNewContributionNotification(Conversation contributions)
        {
            CreateNewConversationWindow(contributions);
        }

        private void StartNewMultiUserConversation()
        {
            // TODO: Create new multi user conversation
        }

        private bool CanStartNewMultiUserConversation()
        {
            return connectedUsers.Any(connectedUser => connectedUser.IsSelectedForConversation);
        }

        private static void CreateNewConversationWindow(Conversation conversation)
        {
            // Check if conversation window already exists
            if (ConversationWindowsStatusCollection.GetWindowStatus(conversation.ConversationId) == WindowStatus.Closed)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var chatWindow = new ChatWindow(conversation);
                    chatWindow.Show();
                });

                ConversationWindowsStatusCollection.SetWindowStatus(conversation.ConversationId, WindowStatus.Open);
            }
        }

        private void OnNewUser(IEnumerable<User> newUser)
        {
            List<User> newUserList = newUser.Where(user => user.UserId != Client.ClientUserId).ToList();

            List<ConnectedUserViewModel> users = newUserList.Select(user => new ConnectedUserViewModel(user)).ToList();

            ConnectedUsers = users;
        }

        public void NewConversation(int secondParticipantUserID)
        {
            foreach (Conversation conversation in Client.ConversationRepository.GetAllEntities()
                .Where(conversation => (Client.ClientUserId == conversation.FirstParticipantUserId ||
                                        Client.ClientUserId == conversation.SecondParticipantUserId) &&
                                       (secondParticipantUserID == conversation.FirstParticipantUserId ||
                                        secondParticipantUserID == conversation.SecondParticipantUserId)))
            {
                OnNewConversationNotification(conversation);
                return;
            }

            Client.SendConversationRequest(secondParticipantUserID);
        }
    }
}