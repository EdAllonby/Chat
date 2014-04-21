using System.Collections.ObjectModel;
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
                viewModel.Contributions = new ObservableCollection<Contribution>(conversation.Contributions);
            }
        }
    }
}