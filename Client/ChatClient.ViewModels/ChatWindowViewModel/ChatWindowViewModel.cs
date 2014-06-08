using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ChatClient.Models.ChatModel;
using ChatClient.Services;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.ChatWindowViewModel
{
    public sealed class ChatWindowViewModel : ViewModel, IDisposable
    {
        private readonly IAudioPlayer audioPlayer = new AudioPlayer();
        private readonly IClientService clientService = ServiceManager.GetService<IClientService>();
        private readonly RepositoryManager repositoryManager;

        private GroupChatModel groupChat = new GroupChatModel();

        public ChatWindowViewModel()
        {
            // Default constructor used for WPF design time view.
        }

        public ChatWindowViewModel(Conversation conversation)
        {
            repositoryManager = clientService.RepositoryManager;
            groupChat.Conversation = conversation;
            groupChat.WindowTitle = repositoryManager.UserRepository.FindUserByID(clientService.ClientUserId).Username;
            groupChat.Title = GetChatTitle();
            groupChat.Users = GetUsers();

            clientService.NewContributionNotification += NewContributionNotificationReceived;
        }

        public GroupChatModel GroupChat
        {
            get { return groupChat; }
            set
            {
                if (Equals(value, groupChat)) return;
                groupChat = value;
                OnPropertyChanged();
            }
        }

        private List<User> GetUsers()
        {
            ParticipationRepository participationRepository = clientService.RepositoryManager.ParticipationRepository;
            UserRepository userRepository = clientService.RepositoryManager.UserRepository;

            return participationRepository.GetParticipationsByConversationId(groupChat.Conversation.ConversationId)
                .Select(participation => userRepository.FindUserByID(participation.UserId)).ToList();
        }

        #region Commands

        public ICommand SendMessage
        {
            get { return new RelayCommand(NewConversationContributionRequest, CanSendConversationContributionRequest); }
        }

        public ICommand Closing
        {
            get { return new RelayCommand(() => ConversationWindowsStatusCollection.SetWindowStatus(groupChat.Conversation.ConversationId, WindowStatus.Closed)); }
        }

        private void NewConversationContributionRequest()
        {
            clientService.SendContributionRequest(groupChat.Conversation.ConversationId, groupChat.MessageToAddToConversation);

            groupChat.MessageToAddToConversation = string.Empty;
        }

        private bool CanSendConversationContributionRequest()
        {
            return !String.IsNullOrEmpty(groupChat.MessageToAddToConversation);
        }

        #endregion

        public void Dispose()
        {
            audioPlayer.Dispose();
        }

        private string GetChatTitle()
        {
            var titleBuilder = new StringBuilder();
            titleBuilder.Append("Chat between ");

            foreach (Participation participant in repositoryManager.ParticipationRepository.GetAllParticipations()
                .Where(participant => participant.ConversationId == groupChat.Conversation.ConversationId))
            {
                titleBuilder.Append(repositoryManager.UserRepository.FindUserByID(participant.UserId).Username);
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
            if (updatedConversation.ConversationId == groupChat.Conversation.ConversationId)
            {
                Application.Current.Dispatcher.Invoke(GetMessages);

                if (groupChat.Conversation.GetAllContributions().Last().ContributorUserId != clientService.ClientUserId)
                {
                    audioPlayer.Play(Resources.Chat_Notification_Sound);
                }
            }
        }

        private void GetMessages()
        {
            IEnumerable<Contribution> contributions = groupChat.Conversation.GetAllContributions();

            var userMessages = new List<UserMessageModel>();

            foreach (Contribution contribution in contributions)
            {
                string message = contribution.Message;

                var messageDetails = new StringBuilder();
                messageDetails.Append(repositoryManager.UserRepository.FindUserByID(contribution.ContributorUserId).Username);
                messageDetails.Append(" sent at: ");
                messageDetails.Append(contribution.MessageTimeStamp.ToString("HH:mm:ss dd/MM/yyyy", new CultureInfo("en-GB")));

                userMessages.Add(new UserMessageModel(message, messageDetails.ToString()));
            }

            groupChat.UserMessages = userMessages;
        }
    }
}