using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using ChatClient.Annotations;
using ChatClient.Commands;
using ChatClient.Views;
using log4net;

namespace ChatClient.ViewModels
{
    internal class LoginWindowViewModel : ViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginWindowViewModel));

        private readonly ClientLoginParser loginParser = new ClientLoginParser();

        private ChatWindow chatWindow;

        [UsedImplicitly] private Client client;

        private string ipAddress = "IP Address";
        private string port = "Port";
        private string username = "Username";

        public LoginWindowViewModel()
        {
            var commandLineArgs = new List<string>(Environment.GetCommandLineArgs());

            commandLineArgs.RemoveAt(0);

            if (commandLineArgs.Count != 0)
            {
                Log.Info("Command line arguments found, attempting to parse");
                bool result = loginParser.ParseCommandLineArguments(Environment.GetCommandLineArgs());
                if (result)
                {
                    AttemptLogin();
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

        private void ParseDetails()
        {
            bool result = loginParser.ParseLogonDetails(username, ipAddress, port);
            if (result)
            {
                AttemptLogin();
            }
            else
            {
                MessageBox.Show("One or more entries were invalid, double check the formatting");
            }
        }

        private void AttemptLogin()
        {
            try
            {
                client = Client.GetInstance(loginParser.Username, loginParser.TargetedAddress, loginParser.TargetedPort);
                OpenChatWindow();
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

        private void OpenChatWindow()
        {
            chatWindow = new ChatWindow();
            Application.Current.MainWindow.Close();
            chatWindow.Show();
        }

        #region Commands

        #region Login Command

        public ICommand Login
        {
            get { return new RelayCommand(LoginToChat, CanLogin); }
        }

        private void LoginToChat()
        {
            ParseDetails();
            OpenChatWindow();
        }

        private bool CanLogin()
        {
            return !(String.IsNullOrEmpty(Username) || String.IsNullOrEmpty(IPAddress) || String.IsNullOrEmpty(Port));
        }

        #endregion

        #region Close Command

        public ICommand CloseWindow
        {
            get { return new RelayCommand(() => Application.Current.MainWindow.Close()); }
        }

        #endregion

        #endregion
    }
}