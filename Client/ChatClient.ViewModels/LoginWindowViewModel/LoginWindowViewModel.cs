using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ChatClient.Models.LoginModel;
using ChatClient.Services;
using ChatClient.ViewModels.Commands;
using log4net;
using SharedClasses;
using SharedClasses.Message;

namespace ChatClient.ViewModels.LoginWindowViewModel
{
    public class LoginWindowViewModel : ViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginWindowViewModel));

        private readonly IClientService clientService;
        private readonly ClientLogOnParser logOnParser = new ClientLogOnParser();
        private bool canOpenWindow;

        public EventHandler<LoginErrorEventArgs> LoginErrored;

        private LoginModel loginModel = new LoginModel();
        public EventHandler OpenMainWindowRequested;

        public LoginWindowViewModel(IServiceRegistry serviceRegistry)
            : base(serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                clientService = serviceRegistry.GetService<IClientService>();

                clientService.BootstrapCompleted += OnClientBootstrapCompleted;
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
            LoginResult result = clientService.LogOn(loginDetails);

            switch (result)
            {
                case LoginResult.Success:
                    Log.Debug("Waiting for client bootstrap to complete");
                    canOpenWindow = true;
                    break;

                case LoginResult.AlreadyConnected:
                    LoginErrored(this, new LoginErrorEventArgs(result, $"User already connected with username: {LoginModel.Username}"));
                    break;

                case LoginResult.ServerNotFound:
                    LoginErrored(this, new LoginErrorEventArgs(result, "Could not find server, check connection settings."));
                    break;
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

        public ICommand Login => new RelayCommand(LoginToChat, CanLogin);

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
            return string.IsNullOrWhiteSpace(LoginModel.Error);
        }

        #endregion

        #endregion
    }
}