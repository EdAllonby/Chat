using System.ComponentModel;
using ChatClient.ViewModels;
using SharedClasses.Domain;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        public ChatWindow(Conversation conversation)
        {
            InitializeComponent();

            var viewModel = DataContext as ChatWindowViewModel;

            if (viewModel != null)
            {
                viewModel.Conversation = conversation;
                viewModel.InitialiseChat();
            }
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