using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            else
            {
                Client.OnNewUser += OnNewUser;
            }
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

        private void OnNewUser(IList<User> newUser, EventArgs e)
        {
            Users = new ObservableCollection<User>(newUser);
        }
    }
}