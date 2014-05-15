using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using ChatClient.Properties;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    public class ChatWindowViewModel : ViewModel
    {
        private readonly IAudioPlayer audioPlayer = new AudioPlayer();
        private Conversation conversation;
        private string messageToAddToConversation;
        private IList<UserMessageViewModel> messages = new List<UserMessageViewModel>();
        private string title;
        private string windowTitle;

        public ChatWindowViewModel()
        {
            windowTitle = Client.Username;
            Client.OnNewContributionNotification += NewContributionNotificationReceived;
        }

        public string WindowTitle
        {
            get { return windowTitle; }
            set
            {
                if (value == windowTitle) return;
                windowTitle = value;
                OnPropertyChanged();
            }
        }

        public Conversation Conversation
        {
            set
            {
                if (Equals(value, conversation)) return;
                conversation = value;
                OnPropertyChanged();
                Title = "Chat between " + Client.UserRepository.FindEntityByID(conversation.FirstParticipantUserId).Username + " and " +
                        Client.UserRepository.FindEntityByID(conversation.SecondParticipantUserId).Username;
            }
        }

        public IList<UserMessageViewModel> Messages
        {
            get { return messages; }
            set
            {
                if (Equals(value, messages)) return;
                messages = value;
                OnPropertyChanged();
            }
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

        public string MessageToAddToConversation
        {
            get { return messageToAddToConversation; }
            set
            {
                if (value == messageToAddToConversation) return;
                messageToAddToConversation = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand SendMessage
        {
            get { return new RelayCommand(NewConversationContributionRequest, CanSendConversationContributionRequest); }
        }

        public ICommand Closing
        {
            get { return new RelayCommand(() => ConversationWindowsStatus.SetWindowStatus(conversation.ConversationId, WindowStatus.Closed)); }
        }

        private void NewConversationContributionRequest()
        {
            Client.SendContributionRequest(conversation.ConversationId, MessageToAddToConversation);

            messageToAddToConversation = string.Empty;
            OnPropertyChanged("MessageToAddToConversation");
        }

        private bool CanSendConversationContributionRequest()
        {
            return !String.IsNullOrEmpty(MessageToAddToConversation);
        }

        #endregion

        public void InitialiseChat()
        {
            GetMessages();
        }

        private void NewContributionNotificationReceived(Conversation updatedConversation)
        {
            if (updatedConversation.ConversationId == conversation.ConversationId)
            {
                Application.Current.Dispatcher.Invoke(GetMessages);

                if (conversation.GetAllContributions().Last().ContributorUserId != Client.ClientUserId)
                {
                    audioPlayer.Play(Resources.Chat_Notification_Sound);
                }
            }
        }

        private void GetMessages()
        {
            IEnumerable<Contribution> contributions = conversation.GetAllContributions();

            var userMessages = new List<UserMessageViewModel>();

            foreach (Contribution contribution in contributions)
            {
                var messageDetails = new StringBuilder();
                messageDetails.Append(Client.UserRepository.FindEntityByID(contribution.ContributorUserId).Username);
                messageDetails.Append(" sent at: ");
                messageDetails.Append(contribution.MessageTimeStamp.ToString("HH:mm:ss dd/MM/yyyy", new CultureInfo("en-GB")));

                string message = contribution.Message;

                userMessages.Add(new UserMessageViewModel(message, messageDetails.ToString()));
            }

            Messages = userMessages;
        }
    }
}