using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using ChatClient.Annotations;
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

        private bool alreadyFaded;
        private ChatWindow chatWindow;
        [UsedImplicitly] private Client client;

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

            if (focusedTextBox.Text == "Username")
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
                    lostFocusTextBox.Text = "Username";
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
            ParseDetails();
        }

        private void ParseDetails()
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
                chatWindow = new ChatWindow();
                Close();
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

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ParseDetails();
            }
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region Close Logic

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            alreadyFaded = false;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!alreadyFaded)
            {
                alreadyFaded = true;
                e.Cancel = true;
                var closingAnimation = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
                closingAnimation.Completed += ClosingAnimationOnCompleted;
                BeginAnimation(OpacityProperty, closingAnimation);
            }
        }

        private void ClosingAnimationOnCompleted(object sender, EventArgs e)
        {
            Close();
            if (chatWindow != null)
            {
                chatWindow.Show();
            }
            else
            {
                Log.Warn("Chat window cannot be opened");
            }
        }

        #endregion
    }
}