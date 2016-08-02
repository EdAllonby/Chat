using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using ChatClient.Services;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Properties;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class MainWindowViewModel : ViewModel
    {
        private readonly int userId;
        private readonly IReadOnlyEntityRepository<User> userRepository;
        public EventHandler OpenUserSettingsWindowRequested;
        private Image userAvatar = Resources.DefaultUserImage;

        public MainWindowViewModel(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                userRepository = ServiceRegistry.GetService<RepositoryManager>().GetRepository<User>();
                userId = ServiceRegistry.GetService<IClientService>().ClientUserId;

                User user = userRepository.FindEntityById(userId);
                if (user.Avatar.UserAvatar != null)
                {
                    UserAvatar = user.Avatar.UserAvatar;
                }

                userRepository.EntityUpdated += OnUserUpdated;
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

        public string Username => userRepository.FindEntityById(userId).Username;

        public ICommand OpenUserSettings => new RelayCommand(OpenUserSettingsWindow);

        private void OpenUserSettingsWindow()
        {
            Application.Current.Dispatcher.Invoke(OnOpenUserSettingsWindowRequested);
        }

        private void OnUserUpdated(object sender, EntityChangedEventArgs<User> e)
        {
            if (e.Entity.Id == userId && !e.PreviousEntity.Avatar.Equals(e.Entity.Avatar))
            {
                UserAvatar = e.Entity.Avatar.UserAvatar;
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