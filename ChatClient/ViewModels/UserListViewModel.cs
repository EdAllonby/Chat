using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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

            Username = Client.UserName;
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
            Application.Current.Dispatcher.Invoke(delegate
            {
                var chatWindow = new ChatWindow(conversation);
                chatWindow.Show();
            });
        }

        private void OnNewUser(IList<User> newUser, EventArgs e)
        {
            List<User> newUserList = newUser.Where(user => user.UserId != Client.ClientUserId).ToList();

            Users = newUserList;
        }

        public void NewConversation(int secondParticipantUserID)
        {
            foreach (var conversation in Client.ConversationRepository.GetAllConversations()
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