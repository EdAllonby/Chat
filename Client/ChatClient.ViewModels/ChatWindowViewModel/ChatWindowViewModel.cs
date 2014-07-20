using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using ChatClient.Models.ChatModel;
using ChatClient.Models.ChatWindowViewModel;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.ChatWindowViewModel
{
    public sealed class ChatWindowViewModel : ViewModel, IDisposable
    {
        private readonly IAudioPlayer audioPlayer = new AudioPlayer();
        private readonly RepositoryManager repositoryManager;
        private readonly ContributionMessageFormatter contributionMessageFormatter;

        public EventHandler OpenUserSettingsWindowRequested;

        private List<ConnectedUserModel> connectedUsers = new List<ConnectedUserModel>();
        private GroupChatModel groupChat = new GroupChatModel();

        public ChatWindowViewModel()
        {
            // Default constructor used for WPF design time view.
        }

        public ChatWindowViewModel(Conversation conversation)
        {
            if (!IsInDesignMode)
            {
                contributionMessageFormatter = new ContributionMessageFormatter(ClientService.ClientUserId, ClientService.RepositoryManager.UserRepository);

                repositoryManager = ClientService.RepositoryManager;

                repositoryManager.UserRepository.UserChanged += OnUserChanged;

                repositoryManager.ConversationRepository.ConversationUpdated += OnConversationUpdated;
                repositoryManager.ConversationRepository.ContributionAdded += OnContributionAdded;

                AddUserCommand = new AddUserToConversationCommand(this);

                groupChat.Conversation = conversation;
                groupChat.Users = GetUsers();

                UpdateConnectedUsersList();

                groupChat.WindowTitle = repositoryManager.UserRepository.FindUserById(ClientService.ClientUserId).Username;
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

        private List<User> GetUsers()
        {
            ParticipationRepository participationRepository = ClientService.RepositoryManager.ParticipationRepository;
            UserRepository userRepository = ClientService.RepositoryManager.UserRepository;

            return participationRepository.GetParticipationsByConversationId(groupChat.Conversation.ConversationId)
                .Select(participation => userRepository.FindUserById(participation.UserId)).ToList();
        }

        private string GetChatTitle()
        {
            var usernames = new List<string>();

            var titleBuilder = new StringBuilder();
            titleBuilder.Append("Chat between ");

            foreach (Participation participant in repositoryManager.ParticipationRepository.GetParticipationsByConversationId(groupChat.Conversation.ConversationId))
            {
                usernames.Add(repositoryManager.UserRepository.FindUserById(participant.UserId).Username);
            }

            titleBuilder.Append(TitleBuilder.CreateUserList(usernames));

            return titleBuilder.ToString();
        }

        private void UpdateConnectedUsersList()
        {
            IEnumerable<User> users = repositoryManager.UserRepository.GetAllUsers();

            List<User> newUserList = users.Where(user => user.UserId != ClientService.ClientUserId)
                .Where(user => user.ConnectionStatus.UserConnectionStatus == ConnectionStatus.Status.Connected).ToList();

            List<ConnectedUserModel> otherUsers = newUserList.Select(user => new ConnectedUserModel(user)).ToList();

            ConnectedUsers = otherUsers;
        }

        public void InitialiseChat()
        {
            GetMessages();
        }

        private void OnContributionAdded(object sender, Contribution contribution)
        {
            if (contribution.ConversationId == groupChat.Conversation.ConversationId)
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var messages = GroupChat.Messages;
                    messages.Blocks.Add(contributionMessageFormatter.FormatContribution(contribution));
                    GroupChat.Messages = messages;
                });

                if (groupChat.Conversation.GetAllContributions().Last().ContributorUserId != ClientService.ClientUserId)
                {
                    audioPlayer.Play(Resources.Chat_Notification_Sound);
                }
            }
        }

        void OnUserChanged(object sender, EntityChangedEventArgs<User> e)
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

        private void OnConversationUpdated(object sender, Conversation conversation)
        {
            // The model is no longer referencing the same conversation as in the repository, give it the reference again.
            groupChat.Conversation = repositoryManager.ConversationRepository.FindConversationById(conversation.ConversationId);

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
            get { return new RelayCommand(() => ConversationWindowManager.SetWindowStatus(groupChat.Conversation.ConversationId, WindowStatus.Closed)); }
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
                ClientService.AddUserToConversation(selectedUser.UserId, GroupChat.Conversation.ConversationId);
            }
        }

        public bool CanAddUser(object user)
        {
            var connectedUser = (ConnectedUserModel) user;

            IEnumerable<Participation> participations = ClientService.RepositoryManager.ParticipationRepository
                .GetParticipationsByConversationId(groupChat.Conversation.ConversationId);

            return participations.All(participation => participation.UserId != connectedUser.UserId);
        }

        private bool CanSendConversationContributionRequest()
        {
            return !String.IsNullOrEmpty(groupChat.MessageToAddToConversation);
        }

        private void NewConversationContributionRequest()
        {
            ClientService.SendContribution(groupChat.Conversation.ConversationId, groupChat.MessageToAddToConversation);

            groupChat.MessageToAddToConversation = string.Empty;
        }

        #endregion
    }
}