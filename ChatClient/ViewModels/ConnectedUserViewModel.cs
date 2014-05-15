using SharedClasses.Domain;

namespace ChatClient.ViewModels
{
    /// <summary>
    /// Translates the <see cref="User"/> model for the View to use
    /// </summary>
    public sealed class ConnectedUserViewModel : ViewModel
    {
        private readonly User user;
        private bool multiUserSelectionMode;
        private bool isSelectedForConversation;

        public ConnectedUserViewModel(User user)
        {
            this.user = user;
        }

        public bool IsSelectedForConversation
        {
            get { return isSelectedForConversation; }
            set
            {
                if (value.Equals(isSelectedForConversation)) return;
                isSelectedForConversation = value;
                OnPropertyChanged();
            }
        }

        public bool MultiUserSelectionMode
        {
            get { return multiUserSelectionMode; }
            set
            {
                if (Equals(value, multiUserSelectionMode)) return;
                multiUserSelectionMode = value;
                if (value == false)
                {
                    IsSelectedForConversation = false;
                }
                OnPropertyChanged();
            }
        }

        public int UserId
        {
            get { return user.UserId; }
        }

        public string Username
        {
            get { return user.Username; }
        }
    }
}