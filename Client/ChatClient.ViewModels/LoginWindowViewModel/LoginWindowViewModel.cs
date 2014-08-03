using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Input;
using ChatClient.Models.LoginModel;
using ChatClient.Services;
using ChatClient.ViewModels.Commands;
using log4net;
using SharedClasses.Message;

namespace ChatClient.ViewModels.LoginWindowViewModel
{
    public class LoginWindowViewModel : ViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginWindowViewModel));

        private readonly ClientLogOnParser logOnParser = new ClientLogOnParser();
        public EventHandler OpenMainWindowRequested;

        private bool canOpenWindow;

        private LoginModel loginModel = new LoginModel();

        public LoginWindowViewModel()
        {
            if (!IsInDesignMode)
            {
                ClientService.BootstrapCompleted += OnClientBootstrapCompleted;
                var commandLineArgs = new List<string>(Environment.GetCommandLineArgs());

                commandLineArgs.RemoveAt(0);

                if (commandLineArgs.Count != 0)
                {
                    Log.Info("Command line arguments found, attempting to parse");

                    LoginDetails loginDetails;
                    bool result = logOnParser.TryParseCommandLineArguments(Environment.GetCommandLineArgs(), out loginDetails);
                    if (result)
                    {
                        AttemptLogin(loginDetails);
                    }
                }
            }
        }

        public LoginModel LoginModel
        {
            get { return loginModel; }
            set
            {
                if (Equals(value, loginModel)) return;
                loginModel = value;
                OnPropertyChanged();
            }
        }

        private void AttemptLogin(LoginDetails loginDetails)
        {
            try
            {
                LoginResult result = ClientService.LogOn(loginDetails);

                switch (result)
                {
                    case LoginResult.Success:
                        Log.Debug("Waiting for client bootstrap to complete");
                        canOpenWindow = true;
                        break;

                    case LoginResult.AlreadyConnected:
                        MessageBox.Show(string.Format("User already connected with username: {0}", LoginModel.Username), "Login Denied");
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
            Application.Current.Dispatcher.Invoke(OnOpenMainWindowRequested);
        }

        private void OnOpenMainWindowRequested()
        {
            EventHandler openMainWindowRequestedCopy = OpenMainWindowRequested;

            if (openMainWindowRequestedCopy != null)
            {
                openMainWindowRequestedCopy(this, EventArgs.Empty);
            }
        }

        private void OnClientBootstrapCompleted(object sender, EventArgs e)
        {
            if (canOpenWindow)
            {
                OpenUserListWindow();
            }
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
            bool result = logOnParser.TryParseLogonDetails(LoginModel.Username, LoginModel.IPAddress, LoginModel.Port, out loginDetails);
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
            return (String.IsNullOrWhiteSpace(LoginModel.Error));
        }

        #endregion

        #endregion
    }
}