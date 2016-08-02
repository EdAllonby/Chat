using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ChatClient.Services;
using ChatClient.ViewModels.Commands;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    /// <summary>
    /// Holds the logic for the view. Accesses the Service manager to receive and send messages.
    /// </summary>
    public class UserListViewModel : ViewModel
    {
        private readonly IClientService clientService;
        private readonly IReadOnlyEntityRepository<Conversation> conversationRepository;
        private readonly ParticipationRepository participationRepository;
        private readonly IReadOnlyEntityRepository<User> userRepository;
        private IList<ConnectedUserViewModel> connectedUsers = new List<ConnectedUserViewModel>();
        private bool isMultiUserConversation;
        private string userFilter = string.Empty;

        public UserListViewModel(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                userRepository = serviceRegistry.GetService<RepositoryManager>().GetRepository<User>();
                conversationRepository = serviceRegistry.GetService<RepositoryManager>().GetRepository<Conversation>();
                participationRepository = (ParticipationRepository) serviceRegistry.GetService<RepositoryManager>().GetRepository<Participation>();
                clientService = serviceRegistry.GetService<IClientService>();
                userRepository.EntityAdded += OnUserChanged;
                userRepository.EntityUpdated += OnUserChanged;

                conversationRepository.EntityAdded += OnConversationAdded;
                conversationRepository.EntityUpdated += OnConversationUpdated;

                UpdateConnectedUsers();
            }
        }

        public bool IsMultiUserConversation
        {
            get { return isMultiUserConversation; }
            set
            {
                foreach (ConnectedUserViewModel connectedUser in ConnectedUsers)
                {
                    connectedUser.MultiUserSelectionMode = value;
                }

                if (Equals(value, isMultiUserConversation))
                {
                    return;
                }

                isMultiUserConversation = value;
                OnPropertyChanged();
            }
        }

        public IList<ConnectedUserViewModel> ConnectedUsers
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

        public string UserFilter
        {
            get { return userFilter; }
            set
            {
                userFilter = value;
                OnPropertyChanged();
                UpdateConnectedUsers();
            }
        }

        public ICommand StartMultiUserConversation => new RelayCommand(StartNewMultiUserConversation, CanStartNewMultiUserConversation);

        private void OnConversationAdded(object sender, EntityChangedEventArgs<Conversation> e)
        {
            ConversationWindowManager.CreateConversationWindow(ServiceRegistry, e.Entity);
        }

        private void OnConversationUpdated(object sender, EntityChangedEventArgs<Conversation> e)
        {
            if (!e.Entity.LastContribution.Equals(e.PreviousEntity.LastContribution))
            {
                OnContributionAdded(e.Entity.LastContribution);
            }
        }

        private void OnContributionAdded(IContribution contribution)
        {
            Conversation conversation = conversationRepository.FindEntityById(contribution.ConversationId);

            ConversationWindowManager.CreateConversationWindow(ServiceRegistry, conversation);
        }

        public void StartNewSingleUserConversation(int participant)
        {
            var participantIds = new List<int> { clientService.ClientUserId, participant };

            NewConversation(participantIds);
        }

        private void StartNewMultiUserConversation()
        {
            var participantIds = new List<int> { clientService.ClientUserId };

            participantIds.AddRange(connectedUsers.Where(user => user.IsSelectedForConversation)
                .Select(connectedUser => connectedUser.UserId));

            NewConversation(participantIds);
        }

        private bool CanStartNewMultiUserConversation()
        {
            return connectedUsers.Any(connectedUser => connectedUser.IsSelectedForConversation);
        }

        private void OnUserChanged(object sender, EntityChangedEventArgs<User> e)
        {
            UpdateConnectedUsers();
        }

        private void UpdateConnectedUsers()
        {
            IEnumerable<User> allUsers = userRepository.GetAllEntities();
            IEnumerable<User> otherUsers = allUsers.Where(user => user.Id != clientService.ClientUserId);
            IEnumerable<User> filteredOtherUsers = otherUsers.Where(CanPresentUser);
            IEnumerable<ConnectedUserViewModel> otherConnectedUsers = filteredOtherUsers.Select(user => new ConnectedUserViewModel(ServiceRegistry, user));

            ConnectedUsers = otherConnectedUsers.ToList();
        }

        private bool CanPresentUser(User unfilteredUsers)
        {
            string[] individualWordFilters = Regex.Split(UserFilter, @"\s");
            string[] usernameWords = Regex.Split(unfilteredUsers.Username, @"\s");

            foreach (string individualWordFilter in individualWordFilters)
            {
                bool isWordFilterValid = usernameWords.Any(usernameWord => usernameWord.StartsWith(individualWordFilter, StringComparison.InvariantCultureIgnoreCase));

                if (!isWordFilterValid)
                {
                    return false;
                }
            }

            return true;
        }

        private void NewConversation(List<int> userIds)
        {
            IsMultiUserConversation = false;

            if (!participationRepository.DoesConversationWithUsersExist(userIds))
            {
                clientService.CreateConversation(userIds);
            }
            else
            {
                int conversationId = participationRepository.GetConversationIdByUserIds(userIds);
                ConversationWindowManager.CreateConversationWindow(ServiceRegistry, conversationRepository.FindEntityById(conversationId));
            }
        }
    }
}