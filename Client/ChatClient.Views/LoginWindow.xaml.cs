using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChatClient.ViewModels.LoginWindowViewModel;
using log4net;
using SharedClasses;

namespace ChatClient.Views
{
    public partial class LoginWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (LoginWindow));
        private readonly IServiceRegistry serviceRegistry;

        public LoginWindow(IServiceRegistry serviceRegistry)
        {
            this.serviceRegistry = serviceRegistry;

            var viewModel = new LoginWindowViewModel(serviceRegistry);
            viewModel.OpenMainWindowRequested += OnOpenMainWindowRequested;
            DataContext = viewModel;

            InitializeComponent();
        }

        private void OnOpenMainWindowRequested(object sender, EventArgs e)
        {
            var view = new MainWindow(serviceRegistry);
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
            var focusedTextBox = (TextBox) sender;

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
            var lostFocusTextBox = (TextBox) sender;

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