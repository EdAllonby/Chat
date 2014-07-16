using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class MainWindowViewModel : ViewModel
    {
        private readonly int userId;
        private readonly UserRepository userRepository;
        public EventHandler OpenUserSettingsWindowRequested;
        private Image userAvatar = Resources.DefaultUserImage;

        public MainWindowViewModel()
        {
            if (!IsInDesignMode)
            {
                userRepository = ClientService.RepositoryManager.UserRepository;
                userRepository.UserAvatarUpdated += OnUserAvatarUpdated;
                userId = ClientService.ClientUserId;
            }
        }

        public Image UserAvatar
        {
            get { return userAvatar; }
            set
            {
                userAvatar = value;
                OnPropertyChanged();
            }
        }

        public string Username
        {
            get { return userRepository.FindUserById(userId).Username; }
        }

        public ICommand OpenUserSettings
        {
            get { return new RelayCommand(OpenUserSettingsWindow); }
        }

        private void OpenUserSettingsWindow()
        {
            Application.Current.Dispatcher.Invoke(OnOpenUserSettingsWindowRequested);
        }

        private void OnUserAvatarUpdated(object sender, User user)
        {
            if (user.UserId == ClientService.ClientUserId)
            {
                UserAvatar = user.Avatar.UserAvatar;
            }
        }

        private void OnOpenUserSettingsWindowRequested()
        {
            EventHandler openUserSettingsWindowRequestedCopy = OpenUserSettingsWindowRequested;
            if (openUserSettingsWindowRequestedCopy != null)
            {
                openUserSettingsWindowRequestedCopy(this, EventArgs.Empty);
            }
        }
    }
}