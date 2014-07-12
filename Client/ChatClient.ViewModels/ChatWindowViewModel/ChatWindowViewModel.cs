using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
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
        private readonly IClientService clientService = ServiceManager.GetService<IClientService>();
        private readonly RepositoryManager repositoryManager;

        private List<ConnectedUserModel> connectedUsers = new List<ConnectedUserModel>();
        private GroupChatModel groupChat = new GroupChatModel();

        public ChatWindowViewModel()
        {
            // Default constructor used for WPF design time view.
        }

        public ChatWindowViewModel(Conversation conversation)
        {
            repositoryManager = clientService.RepositoryManager;

            repositoryManager.UserRepository.UserAdded += OnUserAdded;
            repositoryManager.UserRepository.UserUpdated += OnUserUpdated;

            repositoryManager.ConversationRepository.ConversationUpdated += OnConversationUpdated;
            repositoryManager.ConversationRepository.ContributionAdded += OnContributionAdded;

            AddUserCommand = new AddUserToConversationCommand(this);

            groupChat.Conversation = conversation;
            groupChat.Users = GetUsers();

            UpdateConnectedUsersList();

            groupChat.WindowTitle = repositoryManager.UserRepository.FindUserByID(clientService.ClientUserId).Username;
            groupChat.Title = GetChatTitle();
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
            ParticipationRepository participationRepository = clientService.RepositoryManager.ParticipationRepository;
            UserRepository userRepository = clientService.RepositoryManager.UserRepository;

            return participationRepository.GetParticipationsByConversationId(groupChat.Conversation.ConversationId)
                .Select(participation => userRepository.FindUserByID(participation.UserId)).ToList();
        }

        private string GetChatTitle()
        {
            List<string> usernames = new List<string>();

            var titleBuilder = new StringBuilder();
            titleBuilder.Append("Chat between ");

            foreach (Participation participant in repositoryManager.ParticipationRepository.GetParticipationsByConversationId(groupChat.Conversation.ConversationId))
            {
                usernames.Add(repositoryManager.UserRepository.FindUserByID(participant.UserId).Username);
            }

            titleBuilder.Append(TitleBuilder.CreateUserList(usernames));

            return titleBuilder.ToString();
        }

        private void UpdateConnectedUsersList()
        {
            IEnumerable<User> users = repositoryManager.UserRepository.GetAllUsers();

            List<User> newUserList = users.Where(user => user.UserId != clientService.ClientUserId)
                .Where(user => user.ConnectionStatus == ConnectionStatus.Connected).ToList();

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
                Application.Current.Dispatcher.Invoke(GetMessages);

                if (groupChat.Conversation.GetAllContributions().Last().ContributorUserId != clientService.ClientUserId)
                {
                    audioPlayer.Play(Resources.Chat_Notification_Sound);
                }
            }
        }

        private void OnUserAdded(object sender, User e)
        {
            UpdateConnectedUsersList();
        }

        private void OnUserUpdated(object sender, User user)
        {
            UpdateConnectedUsersList();
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
            get { return new RelayCommand(() => WindowManager.SetWindowStatus(groupChat.Conversation.ConversationId, WindowStatus.Closed)); }
        }

        public void AddUser(object user)
        {
            var selectedUser = user as ConnectedUserModel;

            if (selectedUser != null)
            {
                clientService.AddUserToConversation(selectedUser.UserId, GroupChat.Conversation.ConversationId);
            }
        }

        public bool CanAddUser(object user)
        {
            var connectedUser = (ConnectedUserModel) user;

            IEnumerable<Participation> participations = clientService.RepositoryManager.ParticipationRepository
                .GetParticipationsByConversationId(groupChat.Conversation.ConversationId);

            return participations.All(participation => participation.UserId != connectedUser.UserId);
        }

        private bool CanSendConversationContributionRequest()
        {
            return !String.IsNullOrEmpty(groupChat.MessageToAddToConversation);
        }

        private void NewConversationContributionRequest()
        {
            clientService.SendContribution(groupChat.Conversation.ConversationId, groupChat.MessageToAddToConversation);

            groupChat.MessageToAddToConversation = string.Empty;
        }

        #endregion
    }
}