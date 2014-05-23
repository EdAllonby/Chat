using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using ChatClient.Views;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.UserListViewModel
{
    internal class UserListViewModel : ViewModel
    {
        private IList<ConnectedUserViewModel> connectedUsers = new List<ConnectedUserViewModel>();
        private bool isMultiUserConversation;

        public UserListViewModel()
        {
            GetAllUsers(Client.GetAllUsers());

            Client.OnNewUser += OnNewUser;

            Client.OnNewConversationNotification += OnNewConversationNotification;

            Client.OnNewContributionNotification += OnNewContributionNotification;
        }

        public string Username
        {
            get { return Client.GetUser(Client.ClientUserId).Username; }
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

        private void OnNewConversationNotification(Conversation conversation)
        {
            CreateNewConversationWindow(conversation);
        }

        private void OnNewContributionNotification(Conversation contributions)
        {
            CreateNewConversationWindow(contributions);
        }

        public void StartNewSingleUserConversation(int participant)
        {
            var participantIds = new List<int> {Client.ClientUserId, participant};

            NewConversation(participantIds);
        }

        private void StartNewMultiUserConversation()
        {
            var participantIds = new List<int> {Client.ClientUserId};

            participantIds.AddRange(connectedUsers.Where(user => user.IsSelectedForConversation)
                .Select(connectedUser => connectedUser.UserId));

            NewConversation(participantIds);
        }

        private bool CanStartNewMultiUserConversation()
        {
            return connectedUsers.Any(connectedUser => connectedUser.IsSelectedForConversation);
        }

        private void OnNewUser(IEnumerable<User> newUser)
        {
            GetAllUsers(newUser);
        }

        private void GetAllUsers(IEnumerable<User> users)
        {
            List<User> newUserList = users.Where(user => user.UserId != Client.ClientUserId)
                .Where(user => user.ConnectionStatus == ConnectionStatus.Connected).ToList();

            List<ConnectedUserViewModel> otherUsers = newUserList.Select(user => new ConnectedUserViewModel(user)).ToList();

            ConnectedUsers = otherUsers;
        }

        private void NewConversation(List<int> participantIds)
        {
            IsMultiUserConversation = false;

            if (!Client.DoesConversationExist(participantIds))
            {
                Client.SendConversationRequest(participantIds);
            }
            else
            {
                int conversationId = Client.GetConversationId(participantIds);
                CreateNewConversationWindow(Client.GetConversation(conversationId));
            }
        }

        private void CreateNewConversationWindow(Conversation conversation)
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
    }
}