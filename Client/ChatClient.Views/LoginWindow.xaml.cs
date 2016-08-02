using System;
using System.Windows;
using System.Windows.Input;
using ChatClient.ViewModels.LoginWindowViewModel;
using SharedClasses;

namespace ChatClient.Views
{
    public partial class LoginWindow
    {
        private readonly IServiceRegistry serviceRegistry;

        public LoginWindow(IServiceRegistry serviceRegistry)
        {
            this.serviceRegistry = serviceRegistry;

            var viewModel = new LoginWindowViewModel(serviceRegistry);
            viewModel.OpenMainWindowRequested += OnOpenMainWindowRequested;
            viewModel.LoginErrored += OnLoginErrored;
            DataContext = viewModel;

            InitializeComponent();
        }

        private static void OnLoginErrored(object sender, LoginErrorEventArgs e)
        {
            MessageBox.Show(e.ErrorDescription, e.Result.ToString());
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
    }
}