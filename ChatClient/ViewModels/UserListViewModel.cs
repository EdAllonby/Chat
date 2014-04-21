using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ChatClient.Views;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    internal class UserListViewModel : ViewModel
    {
        public readonly Client Client = Client.GetInstance();
        private ObservableCollection<User> users;

        public UserListViewModel()
        {
            // Sometimes the OnNewUser event fires in Client before Application is ready.
            // This workaround below will guarantee that the UserList Window gets an up to date User list.
            if (Client.ConnectedUsers != null)
            {
                users = new ObservableCollection<User>(Client.ConnectedUsers);
            }

            Client.OnNewUser += OnNewUser;

            Client.OnNewConversationNotification += OnNewConversationNotification;
        }

        public ObservableCollection<User> Users
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
            Users = new ObservableCollection<User>(newUser);
        }

        public void NewConversation(int userID)
        {
            Client.SendConversationRequest(userID);
        }
    }
}