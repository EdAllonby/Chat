using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ChatClient.Commands;
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
            NewMessage = new MessagesReceivedList();
            NewMessage.PropertyChanged += NewMessage_PropertyChanged;
        }

        public UserList UserList { get; set; }

        public MessageToSend MessageToSend { get; set; }

        public string MessageToSendToClient { get; set; }

        public MessagesReceivedList NewMessage { get; set; }

        public ICollection<User> Users
        {
            get { return UserList.Users; }
        }

        public ICollection<String> UpdatedMessage
        {
            get { return NewMessage.Messages; }
        }

        private void userModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Users")
            {
                OnPropertyChanged("Users");
            }
        }

        private void NewMessage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Messages")
            {
                OnPropertyChanged("UpdatedMessage");
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

        #region Commands

        public ICommand SendMessage
        {
            get { return new RelayCommand(NewContributionNotification, CanUpdateArtistNameExecute); }
        }

        private void NewContributionNotification()
        {
            MessageToSend = new MessageToSend(MessageToSendToClient);
        }

        private bool CanUpdateArtistNameExecute()
        {
            return true;
        }

        #endregion
    }
}