using System.Net.Mime;
using System.Windows;
using System.Windows.Input;
using ChatClient.ViewMediator;
using ChatClient.ViewModels.Commands;
using ChatClient.ViewModels.Test;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class MainWindowViewModel : ViewModel
    {
        private readonly int userId;
        private readonly UserRepository userRepository;

        public MainWindowViewModel()
        {
            if (!IsInDesignMode)
            {
                userRepository = ClientService.RepositoryManager.UserRepository;
                userId = ClientService.ClientUserId;
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

        private static void OpenUserSettingsWindow()
        {
            Application.Current.Dispatcher.Invoke(
                () => Mediator.Instance.SendMessage(ViewName.UserSettingsWindow, new UserSettingsViewModel()));
        }
    }
}