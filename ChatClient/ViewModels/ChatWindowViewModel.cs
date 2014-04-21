using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    public class ChatWindowViewModel : ViewModel
    {
        public readonly Client Client = Client.GetInstance();

        private string messageToSendToClient;
        private ObservableCollection<Contribution> messages = new ObservableCollection<Contribution>();
        private string title;

        public ChatWindowViewModel()
        {
            Client.OnNewContributionNotification += NewContributionNotification;
            Title = "Welcome to chat, " + Client.UserName;
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

        public ObservableCollection<Contribution> Messages
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
            Client.SendContributionRequest(MessageToSendToClient);

            messageToSendToClient = string.Empty;
            OnPropertyChanged("MessageToSendToClient");
        }

        private bool CanSendContributionRequest()
        {
            return !String.IsNullOrEmpty(MessageToSendToClient);
        }

        #endregion

        private void NewContributionNotification(Contribution contribution, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Messages.Add(contribution));
        }
    }
}