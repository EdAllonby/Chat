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
            get { return userRepository.FindUserByID(userId).Username; }
        }
    }
}