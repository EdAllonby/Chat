using System;
using System.Windows;
using System.Windows.Controls;
using log4net;

namespace ChatClient.Windows
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
                    LoginToChat();
                }
            }
        }

        private void RemoveTextOnFocus(object sender, RoutedEventArgs e)
        {
            var focusedTextBox = (TextBox) sender;

            if (focusedTextBox.Text == "Enter user name")
            {
                focusedTextBox.Text = string.Empty;
                Log.Debug("Text has been removed from Logon textbox");
            }
            if (focusedTextBox.Text == "Enter Server IP")
            {
                focusedTextBox.Text = string.Empty;
                Log.Debug("Text has been removed from Logon textbox");
            }
            if (focusedTextBox.Text == "Enter Server Port")
            {
                focusedTextBox.Text = string.Empty;
                Log.Debug("Text has been removed from Logon textbox");
            }
        }

        private void AddTextOnLostFocus(object sender, RoutedEventArgs e)
        {
            var lostFocusTextBox = (TextBox) sender;

            if (string.IsNullOrEmpty(lostFocusTextBox.Text))
            {
                if (lostFocusTextBox.Name == "LogonNameTextBox")
                {
                    lostFocusTextBox.Text = "Enter user name";
                    Log.Debug("Default text added back to textbox");
                }
                if (lostFocusTextBox.Name == "IPAddressTextBox")
                {
                    lostFocusTextBox.Text = "Enter Server IP";
                    Log.Debug("Default text added back to textbox");
                }
                if (lostFocusTextBox.Name == "PortTextBox")
                {
                    lostFocusTextBox.Text = "Enter Server Port";
                    Log.Debug("Default text added back to textbox");
                }
            }
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            bool result = loginParser.ParseLogonDetails(LogonNameTextBox.Text, IPAddressTextBox.Text, PortTextBox.Text);
            if (result)
            {
                LoginToChat();
            }
            else
            {
                MessageBox.Show("One or more entries were invalid, double check the formatting");
            }
        }

        private void LoginToChat()
        {
            try
            {
                Log.Debug("Logging in to server");
                client = Client.GetInstance(loginParser.Username, loginParser.TargetedAddress, loginParser.TargetedPort);
                var chatWindow = new ChatWindow();
                chatWindow.Show();
                Close();
            }
            catch (TimeoutException timeoutException)
            {
                Log.Error("Cannot find server", timeoutException);
                MessageBox.Show("Could not find server, try again");
            }
        }
    }
}