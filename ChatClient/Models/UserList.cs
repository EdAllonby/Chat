using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChatClient.Properties;
using SharedClasses.Domain;

namespace ChatClient.Models
{
    public class UserList : INotifyPropertyChanged
    {
        private readonly Client client = Client.GetInstance();

        public UserList()
        {
            client.OnNewUser += client_OnNewUser;
        }

        public ObservableCollection<User> Users { get; set; }

        private void client_OnNewUser(IList<User> updatedUsersList, EventArgs e)
        {
            Users = new ObservableCollection<User>(updatedUsersList);
            OnPropertyChanged("Users");
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}