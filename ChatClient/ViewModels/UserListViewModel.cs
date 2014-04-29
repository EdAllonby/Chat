using System;
using System.Collections.Generic;
using System.Windows;
using ChatClient.Views;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    internal class UserListViewModel : ViewModel
    {
        private readonly Client client = Client.GetInstance();
        private IList<User> users = new List<User>();

        public UserListViewModel()
        {
            client.OnNewUser += OnNewUser;

            client.OnNewConversationNotification += OnNewConversationNotification;
        }

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
            Users = newUser;
        }

        public void NewConversation(int userID)
        {
            client.SendConversationRequest(userID);
        }
    }
}