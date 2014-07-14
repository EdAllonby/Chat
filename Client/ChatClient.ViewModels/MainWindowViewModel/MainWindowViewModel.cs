using System.Drawing;
using System.Windows;
using System.Windows.Input;
using ChatClient.ViewMediator;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using ChatClient.ViewModels.Test;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class MainWindowViewModel : ViewModel
    {
        private readonly int userId;
        private readonly UserRepository userRepository;
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

        public static ICommand OpenUserSettings
        {
            get { return new RelayCommand(OpenUserSettingsWindow); }
        }

        private static void OpenUserSettingsWindow()
        {
            Application.Current.Dispatcher.Invoke(
                () => Mediator.Instance.SendMessage(ViewName.UserSettingsWindow, new UserSettingsViewModel()));
        }
        private void OnUserAvatarUpdated(object sender, User user)
        {
            if (user.UserId == ClientService.ClientUserId)
            {
                UserAvatar = user.Avatar.UserAvatar;
            }
        }
    }
}