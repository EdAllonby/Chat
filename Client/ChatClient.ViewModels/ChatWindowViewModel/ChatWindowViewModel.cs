﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ChatClient.Models.ChatModel;
using ChatClient.Models.ChatWindowViewModel;
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
        private readonly IClientService clientService;
        private readonly ContributionMessageFormatter contributionMessageFormatter;

        private readonly IReadOnlyEntityRepository<Conversation> conversationRepository;
        private readonly ParticipationRepository participationRepository;
        private readonly IReadOnlyEntityRepository<User> userRepository;

        public EventHandler OpenUserSettingsWindowRequested;

        private List<ConnectedUserModel> connectedUsers = new List<ConnectedUserModel>();
        private GroupChatModel groupChat = new GroupChatModel();

        public ChatWindowViewModel(Conversation conversation, IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                userRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();
                conversationRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();
                participationRepository = (ParticipationRepository) ServiceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();

                clientService = ServiceRegistry.GetService<IClientService>();

                contributionMessageFormatter = new ContributionMessageFormatter(clientService.ClientUserId, userRepository);

                userRepository.EntityAdded += OnUserChanged;
                userRepository.EntityUpdated += OnUserChanged;

                conversationRepository.EntityUpdated += OnConversationChanged;

                AddUserCommand = new AddUserToConversationCommand(this);

                groupChat.Conversation = conversation;
                groupChat.Users = GetUsers();

                UpdateConnectedUsersList();

                groupChat.WindowTitle = userRepository.FindEntityById(clientService.ClientUserId).Username;
                groupChat.Title = GetChatTitle();
            }
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

        public List<ConnectedUserModel> ConnectedUsers
        {
            get { return connectedUsers; }
            set
            {
                if (Equals(value, connectedUsers))
                {
                    return;
                }

                connectedUsers = value;
                OnPropertyChanged();
            }
        }

        public void Dispose()
        {
            audioPlayer.Dispose();
        }

        private void OnConversationChanged(object sender, EntityChangedEventArgs<Conversation> e)
        {
            if (!e.Entity.LastContribution.Equals(e.PreviousEntity.LastContribution))
            {
                OnContributionAdded(e.Entity.LastContribution);
            }
            else
            {
                OnConversationUpdated(e.Entity);
            }
        }

        private List<User> GetUsers()
        {
            return participationRepository.GetParticipationsByConversationId(groupChat.Conversation.Id)
                .Select(participation => userRepository.FindEntityById(participation.UserId)).ToList();
        }

        private string GetChatTitle()
        {
            var usernames = new List<string>();

            var titleBuilder = new StringBuilder();
            titleBuilder.Append("Chat between ");

            foreach (Participation participant in participationRepository.GetParticipationsByConversationId(groupChat.Conversation.Id))
            {
                usernames.Add(userRepository.FindEntityById(participant.UserId).Username);
            }

            titleBuilder.Append(TitleBuilder.CreateUserList(usernames));

            return titleBuilder.ToString();
        }

        private void UpdateConnectedUsersList()
        {
            IEnumerable<User> users = userRepository.GetAllEntities();

            List<User> newUserList = users.Where(user => user.Id != clientService.ClientUserId)
                .Where(user => user.ConnectionStatus.UserConnectionStatus == ConnectionStatus.Status.Connected).ToList();

            List<ConnectedUserModel> otherUsers = newUserList.Select(user => new ConnectedUserModel(user)).ToList();

            ConnectedUsers = otherUsers;
        }

        public void InitialiseChat()
        {
            GetMessages();
        }

        private void OnContributionAdded(Contribution contribution)
        {
            if (contribution.ConversationId == groupChat.Conversation.Id)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    FlowDocument messages = GroupChat.Messages;
                    messages.Blocks.Add(contributionMessageFormatter.FormatContribution(contribution));
                    GroupChat.Messages = messages;
                });

                if (groupChat.Conversation.LastContribution.ContributorUserId != clientService.ClientUserId)
                {
                    audioPlayer.Play(Resources.Chat_Notification_Sound);
                }
            }
        }

        private void OnUserChanged(object sender, EntityChangedEventArgs<User> e)
        {
            UpdateConnectedUsersList();
        }

        private void GetMessages()
        {
            IEnumerable<Contribution> contributions = groupChat.Conversation.GetAllContributions();

            FlowDocument messages = GroupChat.Messages;

            foreach (Contribution contribution in contributions)
            {
                Paragraph formattedContribution = contributionMessageFormatter.FormatContribution(contribution);
                messages.Blocks.Add(formattedContribution);
            }

            groupChat.Messages = messages;
        }

        private void OnConversationUpdated(IEntity conversation)
        {
            // The model is no longer referencing the same conversation as in the repository, give it the reference again.
            groupChat.Conversation = conversationRepository.FindEntityById(conversation.Id);

            groupChat.Title = GetChatTitle();
        }

        #region Commands

        public ICommand SendMessage
        {
            get { return new RelayCommand(NewConversationContributionRequest, CanSendConversationContributionRequest); }
        }

        public ICommand AddUserCommand { get; private set; }

        public ICommand Closing
        {
            get { return new RelayCommand(() => ConversationWindowManager.SetWindowStatus(groupChat.Conversation.Id, WindowStatus.Closed)); }
        }

        public ICommand OpenUserSettings
        {
            get { return new RelayCommand(OpenUserSettingsWindow); }
        }

        private void OpenUserSettingsWindow()
        {
            Application.Current.Dispatcher.Invoke(OnOpenUserSettingsWindowRequested);
        }

        private void OnOpenUserSettingsWindowRequested()
        {
            EventHandler openUserSettingsWindowRequestedCopy = OpenUserSettingsWindowRequested;
            if (openUserSettingsWindowRequestedCopy != null)
            {
                openUserSettingsWindowRequestedCopy(this, EventArgs.Empty);
            }
        }

        public void AddUser(object user)
        {
            var selectedUser = user as ConnectedUserModel;

            if (selectedUser != null)
            {
                clientService.AddUserToConversation(selectedUser.UserId, GroupChat.Conversation.Id);
            }
        }

        public bool CanAddUser(object user)
        {
            var connectedUser = (ConnectedUserModel) user;

            IEnumerable<Participation> participations = participationRepository.GetParticipationsByConversationId(groupChat.Conversation.Id);

            return participations.All(participation => participation.UserId != connectedUser.UserId);
        }

        private bool CanSendConversationContributionRequest()
        {
            return !String.IsNullOrEmpty(groupChat.MessageToAddToConversation);
        }

        private void NewConversationContributionRequest()
        {
            clientService.SendContribution(groupChat.Conversation.Id, groupChat.MessageToAddToConversation);

            groupChat.MessageToAddToConversation = string.Empty;
        }

        #endregion
    }
}