using System.Drawing;
using SharedClasses.Domain;
using wpfBrush = System.Windows.Media;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    /// <summary>
    /// Translates the <see cref="User"/> model for the View to use
    /// </summary>
    public sealed class ConnectedUserViewModel : ViewModel
    {
        private readonly User user;
        private bool isSelectedForConversation;
        private bool multiUserSelectionMode;

        public ConnectedUserViewModel(User user)
        {
            if (!IsInDesignMode)
            {
                this.user = user;
            }
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

        public System.Windows.Media.Brush BorderColour
        {
            get { return ConnectionStatusToColour(); }
        }

        public Image UserAvatar
        {
            get {
                return user.Avatar != null ? user.Avatar.UserAvatar : null;
            }
        }

        public string Username
        {
            get { return user.Username; }
        }

        private wpfBrush.Brush ConnectionStatusToColour()
        {
            ConnectionStatus connectionStatus = user.ConnectionStatus;
            switch (connectionStatus.UserConnectionStatus)
            {
                case ConnectionStatus.Status.Connected:
                    return wpfBrush.Brushes.Green;
                case ConnectionStatus.Status.Disconnected:
                    return wpfBrush.Brushes.Red;
                default:
                    return wpfBrush.Brushes.Red;
            }
        }
    }
}