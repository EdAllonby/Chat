using System.ComponentModel;
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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var viewModel = DataContext as ChatWindowViewModel;
            if (viewModel != null)
            {
                viewModel.Closing.Execute(null);
            }
        }
    }
}