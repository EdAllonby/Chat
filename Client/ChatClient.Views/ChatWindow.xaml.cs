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

            viewModel.InitialiseChat();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
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