using ChatClient.Services;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.ViewModels.MainWindowViewModel
{
    public sealed class MainWindowViewModel
    {
        private readonly UserRepository userRepository;
        private readonly int userId;

        public MainWindowViewModel()
        {
            IClientService clientService = ServiceManager.GetService<IClientService>();
            userRepository = clientService.RepositoryManager.UserRepository;
            userId = clientService.ClientUserId;
        }

        public string Username
        {
            get { return userRepository.FindUserByID(userId).Username; }
        }
    }
}