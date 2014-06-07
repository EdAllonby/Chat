﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
﻿using ChatClient.Models.Annotations;
﻿using SharedClasses.Domain;

namespace ChatClient.Models
{
    public class GroupChatModel : INotifyPropertyChanged
    {
        private Conversation conversation;
        private string title;
        private List<User> users;
        private string windowTitle;
        private string messageToAddToConversation;
        private List<UserMessageModel> userMessages;

        public List<UserMessageModel> UserMessages
        {
            get { return userMessages; }
            set
            {
                if (Equals(value, userMessages)) return;
                userMessages = value;
                OnPropertyChanged();
            }
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
            get { return conversation; }
            set
            {
                if (Equals(value, conversation)) return;
                conversation = value;
                OnPropertyChanged();
            }
        }

        public List<User> Users
        {
            get { return users; }
            set
            {
                if (Equals(value, users)) return;
                users = value;
                OnPropertyChanged();
            }
        }

        public string Title
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
                if (value == messageToAddToConversation)
                {
                    return;
                }
                messageToAddToConversation = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}