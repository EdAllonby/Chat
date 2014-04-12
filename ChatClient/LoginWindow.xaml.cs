using System;
using System.Windows;
using log4net;

namespace ChatClient
{
    /// <summary>
    ///     Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginWindow));
        public static string[] CommandLineArguments;
        private readonly ClientLoginParser loginParser = new ClientLoginParser();
        private Client client;

        public LoginWindow()
        {
            InitializeComponent();
            if (CommandLineArguments != null && CommandLineArguments.Length != 0)
            {
                Log.Info("Command line arguments found, attempting to parse");
                bool result = loginParser.ParseCommandLineArguments(CommandLineArguments);
                if (result)
                {
                    client = Client.GetInstance(loginParser.Username, loginParser.TargetedAddress, loginParser.TargetedPort);
                    Log.Debug("Command line arguments passed to client object");
                }
            }
        }

        private void RemoveTextOnFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddTextOnLostFocus(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            client = Client.GetInstance();

            throw new NotImplementedException();
        }
    }
}