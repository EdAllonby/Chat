using SharedClasses.Domain;

namespace ChatClient.Models.ChatWindowViewModel
{
    public sealed class ConnectedUserModel
    {
        private readonly User user;

        public ConnectedUserModel(User user)
        {
            this.user = user;
        }

        public int UserId => user.Id;

        public string Username => user.Username;

        public ConnectionStatus ConnectionStatus => user.ConnectionStatus;
    }
}