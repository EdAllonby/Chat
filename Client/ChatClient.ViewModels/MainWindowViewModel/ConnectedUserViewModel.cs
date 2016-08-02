using System.Drawing;
using ChatClient.ViewModels.Properties;
using SharedClasses;
using SharedClasses.Domain;
using wpfBrush = System.Windows.Media;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    /// <summary>
    /// Translates the <see cref="User" /> model for the View to use.
    /// </summary>
    public sealed class ConnectedUserViewModel : ViewModel
    {
        private readonly User user;
        private bool isSelectedForConversation;
        private bool multiUserSelectionMode;

        public ConnectedUserViewModel(IServiceRegistry serviceRegistry, User user)
            : base(serviceRegistry)
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

        public int UserId => user.Id;

        public wpfBrush.Brush BorderColour => ConnectionStatusToColour();

        public Image UserAvatar => user.Avatar.UserAvatar ?? Resources.DefaultUserImage;

        public string Username => user.Username;

        private wpfBrush.Brush ConnectionStatusToColour()
        {
            ConnectionStatus connectionStatus = user.ConnectionStatus;
            switch (connectionStatus.UserConnectionStatus)
            {
                case ConnectionStatus.Status.Connected:
                    return wpfBrush.Brushes.LimeGreen;
                case ConnectionStatus.Status.Disconnected:
                    return wpfBrush.Brushes.Red;
                default:
                    return wpfBrush.Brushes.Red;
            }
        }
    }
}