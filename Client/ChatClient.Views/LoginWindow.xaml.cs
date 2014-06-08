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
    }
}