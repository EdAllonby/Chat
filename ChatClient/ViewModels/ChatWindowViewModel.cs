using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    public class ChatWindowViewModel : ViewModel
    {
        private readonly Client client = Client.GetInstance();

        private string messageToSendToClient;
        private ObservableCollection<string> messages;
        private string title;
        private ObservableCollection<User> users;

        public ChatWindowViewModel()
        {
            client.OnNewUser += client_OnNewUser;
            client.OnNewContributionNotification += client_OnNewContributionNotification;
            Title = "Welcome to chat, " + client.UserName;
            Messages = new ObservableCollection<string>();
        }

        public String Title
        {
            get { return title; }
            set
            {
                if (value == title) return;
                title = value;
                OnPropertyChanged();
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

        public ObservableCollection<String> Messages
        {
            get { return messages; }
            set
            {
                if (Equals(value, messages)) return;
                messages = value;
                OnPropertyChanged();
            }
        }

        public string MessageToSendToClient
        {
            get { return messageToSendToClient; }
            set
            {
                if (value == messageToSendToClient) return;
                messageToSendToClient = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand SendMessage
        {
            get { return new RelayCommand(NewContributionNotification, CanSendContributionRequest); }
        }

        private void NewContributionNotification()
        {
            client.SendContributionRequestMessage(MessageToSendToClient);

            messageToSendToClient = string.Empty;
            OnPropertyChanged("MessageToSendToClient");
        }

        private bool CanSendContributionRequest()
        {
            return !String.IsNullOrEmpty(MessageToSendToClient);
        }

        #endregion

        private void client_OnNewUser(IList<User> users, EventArgs e)
        {
            Users = new ObservableCollection<User>(users);
        }

        private void client_OnNewContributionNotification(string contribution, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Messages.Add(contribution));
        }
    }
}