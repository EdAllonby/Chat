using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using ChatClient.Models.Properties;
using SharedClasses.Domain;

namespace ChatClient.Models.ChatModel
{
    public class GroupChatModel : INotifyPropertyChanged
    {
        private Conversation conversation;
        private FlowDocument messages = new FlowDocument();
        private string messageToAddToConversation;
        private string title;
        private List<User> users;
        private string usersTyping;
        private string windowTitle;

        public GroupChatModel(Participation participation)
        {
            Participation = participation;
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

        public string UsersTyping
        {
            get { return usersTyping; }
            set
            {
                usersTyping = value;
                OnPropertyChanged();
            }
        }

        public Conversation Conversation
        {
            get { return conversation; }
            set
            {
                conversation = value;
                OnPropertyChanged();
            }
        }

        public Participation Participation { get; private set; }

        public FlowDocument Messages
        {
            get { return messages; }
            set
            {
                messages = value;
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
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}