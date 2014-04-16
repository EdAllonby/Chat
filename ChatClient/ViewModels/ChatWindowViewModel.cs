using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using log4net;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    public class ChatWindowViewModel : ViewModel
    {
        private static ILog Log = LogManager.GetLogger(typeof (ChatWindowViewModel));

        private readonly Client client = Client.GetInstance();

        public ChatWindowViewModel()
        {
            client.OnNewUser += client_OnNewUser;
            client.OnNewContributionNotification += client_OnNewContributionNotification;
            Messages = new ObservableCollection<string>();
        }

        public ObservableCollection<User> Users { get; set; }

        public ObservableCollection<String> Messages { get; set; }

        public string MessageToSendToClient { get; set; }

        #region Commands

        public ICommand SendMessage
        {
            get { return new RelayCommand(NewContributionNotification, CanSendContributionRequest); }
        }

        private void NewContributionNotification()
        {
            client.SendContributionRequestMessage(MessageToSendToClient);
        }

        private bool CanSendContributionRequest()
        {
            return !String.IsNullOrEmpty(MessageToSendToClient);
        }

        #endregion

        private void client_OnNewUser(IList<User> users, EventArgs e)
        {
            Users = new ObservableCollection<User>(users);
            OnPropertyChanged("Users");
        }

        private void client_OnNewContributionNotification(string contribution, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Messages.Add(contribution));
            OnPropertyChanged("Messages");
        }
    }
}