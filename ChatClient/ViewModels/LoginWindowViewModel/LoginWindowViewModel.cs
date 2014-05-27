using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using ChatClient.Views;
using log4net;
using SharedClasses.Message;

namespace ChatClient.ViewModels.LoginWindowViewModel
{
    public class LoginWindowViewModel : ViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginWindowViewModel));

        private readonly ClientLoginParser loginParser = new ClientLoginParser();

        private string ipAddress = "IP Address";
        private string port = "Port";
        private UserListWindow userListWindow;
        private string username = "Username";

        public LoginWindowViewModel()
        {
            var commandLineArgs = new List<string>(Environment.GetCommandLineArgs());

            commandLineArgs.RemoveAt(0);

            if (commandLineArgs.Count != 0)
            {
                Log.Info("Command line arguments found, attempting to parse");

                LoginDetails loginDetails;
                bool result = loginParser.TryParseCommandLineArguments(Environment.GetCommandLineArgs(), out loginDetails);
                if (result)
                {
                    AttemptLogin(loginDetails);
                }
            }
        }

        public string Username
        {
            get { return username; }
            set
            {
                if (value == username)
                {
                    return;
                }

                username = value;
                OnPropertyChanged();
            }
        }

        public string IPAddress
        {
            get { return ipAddress; }
            set
            {
                if (value == ipAddress)
                {
                    return;
                }

                ipAddress = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get { return port; }
            set
            {
                if (value == port)
                {
                    return;
                }

                port = value;
                OnPropertyChanged();
            }
        }

        private void AttemptLogin(LoginDetails loginDetails)
        {
            try
            {
                LoginResult result = Client.ConnectToServer(loginDetails);

                switch (result)
                {
                    case LoginResult.Success:
                        OpenUserListWindow();
                        break;
                    
                    case LoginResult.AlreadyConnected:
                        MessageBox.Show(string.Format("User already connected with username: {0}", username), "Login Denied");
                        break;
                }
            }
            catch (TimeoutException timeoutException)
            {
                Log.Error("Cannot find server", timeoutException);
                MessageBox.Show("Could not find server, check the IP Address");
            }
            catch (SocketException socketException)
            {
                Log.Error("Port is incorrect", socketException);
                MessageBox.Show("Could log in to server, check the port");
            }
        }

        private void OpenUserListWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                userListWindow = new UserListWindow();
                Application.Current.MainWindow.Close();
                userListWindow.Show();
            });
        }

        #region Commands

        #region Login Command

        public ICommand Login
        {
            get { return new RelayCommand(LoginToChat, CanLogin); }
        }

        public static ICommand Closing
        {
            get { return new RelayCommand(() => Application.Current.Shutdown()); }
        }

        private void LoginToChat()
        {
            LoginDetails loginDetails;
            bool result = loginParser.TryParseLogonDetails(username, ipAddress, port, out loginDetails);
            if (result)
            {
                AttemptLogin(loginDetails);
            }
            else
            {
                MessageBox.Show("One or more entries were invalid, double check the formatting");
            }
        }

        private bool CanLogin()
        {
            return !(String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(IPAddress) || String.IsNullOrEmpty(Port));
        }

        #endregion

        #endregion
    }
}