using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChatClient.Models;
using ChatClient.Properties;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    public class ChatWindowViewModel : INotifyPropertyChanged
    {
        public ChatWindowViewModel()
        {
            UserList = new UserList();
            UserList.PropertyChanged += userModel_PropertyChanged;
        }

        public UserList UserList { get; set; }

        public ICollection<User> Users2
        {
            get { return UserList.Users; }
            set { OnPropertyChanged(); }
        }

        private void userModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Users")
            {
                OnPropertyChanged("Users2");
            }
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