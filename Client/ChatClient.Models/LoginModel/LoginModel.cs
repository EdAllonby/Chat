using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChatClient.Models.Properties;

namespace ChatClient.Models.LoginModel
{
    public class LoginModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string ipAddress;
        private string port;
        private string username;

        public string Username
        {
            get { return username; }
            set
            {
                if (value == username) return;
                username = value;
                OnPropertyChanged();
            }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set
            {
                if (value == ipAddress) return;
                ipAddress = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get { return port; }
            set
            {
                if (value == port) return;
                port = value;
                OnPropertyChanged();
            }
        }

        public string this[string columnName]
        {
            get
            {
                Error = string.Empty;

                if (string.IsNullOrEmpty(Username))
                {
                    Error = "Username is required.";
                }

                if (string.IsNullOrEmpty(IPAddress))
                {
                    Error = "IP Address is required.";
                }

                int portInt;
                if (!int.TryParse(Port, out portInt))
                {
                    Error = "Port must only consist of digits.";
                }
                if (portInt < 0 || portInt > 65535)
                {
                    Error = "Port must be between 0 and 65535.";
                }

                return Error;
            }
        }

        public string Error { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}