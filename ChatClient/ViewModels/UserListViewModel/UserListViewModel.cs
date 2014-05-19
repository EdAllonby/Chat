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
            GetAllUsers(Client.UserRepository.GetAllEntities());

            Client.OnNewUser += OnNewUser;

            Client.OnNewConversationNotification += OnNewConversationNotification;

            Client.OnNewContributionNotification += OnNewContributionNotification;
        }

        public string Username
        {
            get { return Client.UserRepository.FindEntityByID(Client.ClientUserId).Username; }
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
            List<User> newUserList = users.Where(user => user.UserId != Client.ClientUserId).ToList();

            List<ConnectedUserViewModel> otherUsers = newUserList.Select(user => new ConnectedUserViewModel(user)).ToList();

            ConnectedUsers = otherUsers;
        }

        private void NewConversation(List<int> participantIds)
        {
            IsMultiUserConversation = false;

            bool isNewConversation = CheckConversationExists(participantIds);

            if (isNewConversation)
            {
                Client.SendConversationRequest(participantIds);
            }
        }

        private bool CheckConversationExists(IEnumerable<int> participantIds)
        {
            var userIdsIndexedByConversationId = new Dictionary<int, List<int>>();

            foreach (Participation participation in Client.Participations)
            {
                if (!userIdsIndexedByConversationId.ContainsKey(participation.ConversationId))
                {
                    userIdsIndexedByConversationId[participation.ConversationId] = new List<int>();
                }

                userIdsIndexedByConversationId[participation.ConversationId].Add(participation.UserId);
            }

            foreach (KeyValuePair<int, List<int>> conversationKeyValuePair
                in userIdsIndexedByConversationId.Select(
                    conversationKeyValuePair => new
                    {
                        conversationKeyValuePair,
                        isConversation = conversationKeyValuePair.Value.HasSameElementsAs(participantIds)
                    })
                    .Where(user => user.isConversation)
                    .Select(user => user.conversationKeyValuePair))
            {
                // Conversation has been found, create chat window
                CreateNewConversationWindow(Client.ConversationRepository.FindEntityByID(conversationKeyValuePair.Key));
                return false;
            }

            return true;
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