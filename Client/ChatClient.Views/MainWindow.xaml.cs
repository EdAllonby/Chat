using System;
using System.Windows;
using System.Windows.Controls;
using ChatClient.ViewModels.MainWindowViewModel;
using SharedClasses;

namespace ChatClient.Views
{
    public partial class MainWindow
    {
        private readonly IServiceRegistry serviceRegistry;

        public MainWindow(IServiceRegistry serviceRegistry)
        {
            this.serviceRegistry = serviceRegistry;

            var viewModel = new MainWindowViewModel(serviceRegistry);
            viewModel.OpenUserSettingsWindowRequested += OnOpenUserSettingsWindowRequested;

            InitializeComponent();

            var tabItem1 = (TabItem) Tabs.Items[0];
            tabItem1.Content = new UserListWindow(serviceRegistry);
            var tabItem2 = (TabItem) Tabs.Items[1];
            tabItem2.Content = new ActiveConversations(serviceRegistry);

            DataContext = viewModel;
        }

        private void OnOpenUserSettingsWindowRequested(object sender, EventArgs e)
        {
            var view = new UserSettingsWindow(serviceRegistry);
            view.ShowDialog();
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();

            // Something is stopping the application to close gracefully. Force the application to quit.
            Environment.Exit(0);
        }
    }
}