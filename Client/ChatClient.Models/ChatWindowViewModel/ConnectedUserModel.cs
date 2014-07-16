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

        public int UserId
        {
            get { return user.UserId; }
        }

        public string Username
        {
            get { return user.Username; }
        }

        public ConnectionStatus ConnectionStatus
        {
            get { return user.ConnectionStatus; }
        }
    }
}