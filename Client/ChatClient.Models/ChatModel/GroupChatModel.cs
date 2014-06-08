using System.Collections.Generic;
using SharedClasses.Domain;

namespace ChatClient.Models.ChatModel
{
    public class GroupChatModel : BaseModel
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
    }
}