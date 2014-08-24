﻿using System.Collections.Generic;
using System.Linq;
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

        public ICommand StartMultiUserConversation
        {
            get { return new RelayCommand(StartNewMultiUserConversation, CanStartNewMultiUserConversation); }
        }

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

        private void OnContributionAdded(Contribution contribution)
        {
            Conversation conversation = conversationRepository.FindEntityById(contribution.ConversationId);

            ConversationWindowManager.CreateConversationWindow(ServiceRegistry, conversation);
        }

        public void StartNewSingleUserConversation(int participant)
        {
            var participantIds = new List<int> {clientService.ClientUserId, participant};

            NewConversation(participantIds);
        }

        private void StartNewMultiUserConversation()
        {
            var participantIds = new List<int> {clientService.ClientUserId};

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
            IEnumerable<User> users = userRepository.GetAllEntities();
            List<User> newUserList = users.Where(user => user.Id != clientService.ClientUserId).ToList();

            List<ConnectedUserViewModel> otherUsers = newUserList.Select(user => new ConnectedUserViewModel(ServiceRegistry, user)).ToList();

            ConnectedUsers = otherUsers;
        }

        private void NewConversation(List<int> participantIds)
        {
            IsMultiUserConversation = false;

            if (!participationRepository.DoesConversationWithUsersExist(participantIds))
            {
                clientService.CreateConversation(participantIds);
            }
            else
            {
                int conversationId = participationRepository.GetConversationIdByParticipantsId(participantIds);
                ConversationWindowManager.CreateConversationWindow(ServiceRegistry, conversationRepository.FindEntityById(conversationId));
            }
        }
    }
}