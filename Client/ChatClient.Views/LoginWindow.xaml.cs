using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatClient.ViewMediator;
using ChatClient.ViewModels.UserListViewModel;
using log4net;

namespace ChatClient.Views
{
    public partial class LoginWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginWindow));

        public LoginWindow()
        {
            InitializeComponent();

            Mediator.Instance.Register(ViewName.UserListWindow, ShowUserListWindow);
        }

        private void ShowUserListWindow(object param)
        {
            var view = new UserListWindow((UserListViewModel) param);
            Close();
            view.Show();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
        private void RemoveTextOnFocus(object sender, RoutedEventArgs e)
        {
            var focusedTextBox = (TextBox)sender;

            if (focusedTextBox.Text == "Username")
            {
                focusedTextBox.Text = string.Empty;
                Log.Debug("Text has been removed from LogOn textbox");
            }
            if (focusedTextBox.Text == "IP Address")
            {
                focusedTextBox.Text = string.Empty;
                Log.Debug("Text has been removed from LogOn textbox");
            }
            if (focusedTextBox.Text == "Port")
            {
                focusedTextBox.Text = string.Empty;
                Log.Debug("Text has been removed from LogOn textbox");
            }
        }

        private void AddTextOnLostFocus(object sender, RoutedEventArgs e)
        {
            var lostFocusTextBox = (TextBox)sender;

            if (string.IsNullOrEmpty(lostFocusTextBox.Text))
            {
                if (lostFocusTextBox.Name == "UsernameTextBox")
                {
                    lostFocusTextBox.Text = "Username";
                    Log.Debug("Default text added back to textbox");
                }
                if (lostFocusTextBox.Name == "IPAddressTextBox")
                {
                    lostFocusTextBox.Text = "IP Address";
                    Log.Debug("Default text added back to textbox");
                }
                if (lostFocusTextBox.Name == "PortTextBox")
                {
                    lostFocusTextBox.Text = "Port";
                    Log.Debug("Default text added back to textbox");
                }
            }
        }
    }
}