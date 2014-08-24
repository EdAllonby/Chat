﻿using System.Collections.Generic;
using System.Text;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public class ConversationViewModel : ViewModel
    {
        private readonly Conversation conversation;
        private readonly ParticipationRepository participationRepository;
        private readonly IReadOnlyEntityRepository<User> userRepository;

        public ConversationViewModel(Conversation conversation, IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                this.conversation = conversation;
                userRepository = userRepository;
                participationRepository = participationRepository;
            }
        }

        public string ConversationParticipants
        {
            get { return GetConversationParticipants(); }
        }

        public int ConversationId
        {
            get { return conversation.Id; }
        }

        private string GetConversationParticipants()
        {
            var usernames = new List<string>();

            var titleBuilder = new StringBuilder();

            foreach (Participation participant in participationRepository.GetParticipationsByConversationId(conversation.Id))
            {
                usernames.Add(userRepository.FindEntityById(participant.UserId).Username);
            }

            titleBuilder.Append(TitleBuilder.CreateUserList(usernames));

            return titleBuilder.ToString();
        }
    }
}