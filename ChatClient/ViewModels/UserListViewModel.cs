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
        private IList<User> users = new List<User>();

        public UserListViewModel()
        {
            Client.OnNewUser += OnNewUser;

            Client.OnNewConversationNotification += OnNewConversationNotification;

            Client.OnNewContributionNotification += OnNewContributionNotification;

            Username = Client.Username;
        }

        public string Username { get; private set; }

        public IList<User> Users
        {
            get { return users; }
            set
            {
                if (Equals(value, users)) return;
                users = value;
                OnPropertyChanged();
            }
        }

        private static void OnNewConversationNotification(Conversation conversation)
        {
            CreateNewConversationWindow(conversation);
        }

        private static void OnNewContributionNotification(Conversation contributions)
        {
            CreateNewConversationWindow(contributions);
        }

        private static void CreateNewConversationWindow(Conversation conversation)
        {
            // Check if conversation window already exists
            if (ConversationWindowsStatus.GetWindowStatus(conversation.ConversationId) == WindowStatus.Closed)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    var chatWindow = new ChatWindow(conversation);
                    chatWindow.Show();
                });

                ConversationWindowsStatus.SetWindowStatus(conversation.ConversationId, WindowStatus.Open);
            }
        }

        private void OnNewUser(IEnumerable<User> newUser)
        {
            List<User> newUserList = newUser.Where(user => user.UserId != Client.ClientUserId).ToList();

            Users = newUserList;
        }

        public static ICommand Closing
        {
            get { return new RelayCommand(() => Application.Current.Shutdown()); }
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