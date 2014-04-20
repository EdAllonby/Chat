using System.Windows.Input;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for UserListWindow.xaml
    /// </summary>
    public partial class UserListWindow
    {
        public UserListWindow()
        {
            InitializeComponent();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}