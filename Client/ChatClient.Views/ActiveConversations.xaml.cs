using System.Windows.Controls;
using System.Windows.Input;
using ChatClient.ViewMediator;
using ChatClient.ViewModels.ChatWindowViewModel;
using ChatClient.ViewModels.MainWindowViewModel;

namespace ChatClient.Views
{
    public partial class ActiveConversations
    {
        public ActiveConversations()
        {
            InitializeComponent();
        }

        /// <summary>
        /// I can't find a good way of doing this directly with MVVM and bindings.
        /// Succumbed to creating the click event in the code-behind and then delegating the work off to the viewmodel.
        /// </summary>
        /// <param name="sender">The textblock clicked on</param>
        /// <param name="e">Mouse events for the selected textblock</param>
        private void OnNewConversationSelection(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                var conversationClicked = (StackPanel)sender;
                var conversationId = (int)conversationClicked.Tag;

                var viewmodel = DataContext as ActiveConversationsViewModel;

                if (viewmodel != null)
                {
                    viewmodel.GetConversationWindow(conversationId);
                }
            }
        }
    }
}
