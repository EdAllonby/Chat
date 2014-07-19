using System;
using System.ComponentModel;
using System.Windows.Controls;
using ChatClient.ViewModels.ChatWindowViewModel;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        public ChatWindow(ChatWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            viewModel.OpenUserSettingsWindowRequested += OnOpenUserSettingsWindowRequested;

            viewModel.InitialiseChat();
        }

        private static void OnOpenUserSettingsWindowRequested(object sender, EventArgs e)
        {
            var view = new UserSettingsWindow();
            view.ShowDialog();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            // Cannot directly bind a command to a closing event, so need to call the command in code behind.
            var viewModel = DataContext as ChatWindowViewModel;
            if (viewModel != null)
            {
                viewModel.Closing.Execute(null);
            }
        }

        private void OnMessageTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            MessageTextBox.ScrollToEnd();
        }
    }
}