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

namespace ChatClient.ViewModels.ChatWindowViewModel
{
    public class ChatWindowViewModel : ViewModel
    {
        private readonly IAudioPlayer audioPlayer = new AudioPlayer();
        private IList<UserMessageViewModel> chatMessages = new List<UserMessageViewModel>();
        private string chatTitle;
        private Conversation conversation;
        private string messageToAddToConversation;
        private string windowTitle;

        public ChatWindowViewModel()
        {
            windowTitle = Client.UserRepository.FindEntityByID(Client.ClientUserId).Username;

            Client.OnNewContributionNotification += NewContributionNotificationReceived;
        }

        public string WindowTitle
        {
            get { return windowTitle; }
            set
            {
                if (value == windowTitle)
                {
                    return;
                }

                windowTitle = value;
                OnPropertyChanged();
            }
        }

        public Conversation Conversation
        {
            set
            {
                if (Equals(value, conversation))
                {
                    return;
                }

                conversation = value;

                ChatTitle = GetChatTitle();

                OnPropertyChanged();
            }
        }

        public IList<UserMessageViewModel> ChatMessages
        {
            get { return chatMessages; }
            set
            {
                if (Equals(value, chatMessages))
                {
                    return;
                }

                chatMessages = value;
                OnPropertyChanged();
            }
        }

        public String ChatTitle
        {
            get { return chatTitle; }
            set
            {
                if (value == chatTitle)
                {
                    return;
                }

                chatTitle = value;
                OnPropertyChanged();
            }
        }

        public string MessageToAddToConversation
        {
            get { return messageToAddToConversation; }
            set
            {
                if (value == messageToAddToConversation)
                {
                    return;
                }
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
            get { return new RelayCommand(() => ConversationWindowsStatusCollection.SetWindowStatus(conversation.ConversationId, WindowStatus.Closed)); }
        }

        private void NewConversationContributionRequest()
        {
            Client.SendContributionRequest(conversation.ConversationId, MessageToAddToConversation);

            MessageToAddToConversation = string.Empty;
        }

        private bool CanSendConversationContributionRequest()
        {
            return !String.IsNullOrEmpty(MessageToAddToConversation);
        }

        #endregion

        private string GetChatTitle()
        {
            var titleBuilder = new StringBuilder();
            titleBuilder.Append("Chat between ");

            foreach (Participation participant in Client.ParticipationRepository.GetAllParticipations().Where(participant => participant.ConversationId == conversation.ConversationId))
            {
                titleBuilder.Append(Client.UserRepository.FindEntityByID(participant.UserId).Username);
                titleBuilder.Append(" and ");
            }

            titleBuilder.Length = titleBuilder.Length - " and ".Length;

            string title = titleBuilder.ToString();
            return title;
        }

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
                string message = contribution.Message;

                var messageDetails = new StringBuilder();
                messageDetails.Append(Client.UserRepository.FindEntityByID(contribution.ContributorUserId).Username);
                messageDetails.Append(" sent at: ");
                messageDetails.Append(contribution.MessageTimeStamp.ToString("HH:mm:ss dd/MM/yyyy", new CultureInfo("en-GB")));

                userMessages.Add(new UserMessageViewModel(message, messageDetails.ToString()));
            }

            ChatMessages = userMessages;
        }
    }
}