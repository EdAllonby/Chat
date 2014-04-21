using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    public class ChatWindowViewModel : ViewModel
    {
        private readonly Client client = Client.GetInstance();
        private ObservableCollection<Contribution> contributions;
        private Conversation conversation;
        private string messageToAddToConversation;
        private string title;
        private string windowTitle;


        public ChatWindowViewModel()
        {
            windowTitle = client.UserName;

            client.OnNewContribution += client_OnNewContribution;
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
                Title = "Chat between " + conversation.FirstParticipant.UserName + " and " + conversation.SecondParticipant.UserName;
            }
        }

        public ObservableCollection<Contribution> Contributions
        {
            get { return contributions; }
            set
            {
                if (Equals(value, contributions)) return;
                contributions = value;
                OnPropertyChanged();
            }
        }


        public String Title
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
                if (value == messageToAddToConversation) return;
                messageToAddToConversation = value;
                OnPropertyChanged();
            }
        }

        #region Commands

        public ICommand SendMessage
        {
            get { return new RelayCommand(NewConversationContributionRequest, CanSendConversationContributionRequest); }
        }

        private void NewConversationContributionRequest()
        {
            client.SendConversationContributionRequest(conversation.ID, MessageToAddToConversation);

            messageToAddToConversation = string.Empty;
            OnPropertyChanged("MessageToAddToConversation");
        }

        private bool CanSendConversationContributionRequest()
        {
            return !String.IsNullOrEmpty(MessageToAddToConversation);
        }

        #endregion

        private void client_OnNewContribution(Contribution contribution)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (contribution.Conversation.ID.Equals(conversation.ID))
                {
                    Contributions.Add(contribution);
                }
            });
        }
    }
}