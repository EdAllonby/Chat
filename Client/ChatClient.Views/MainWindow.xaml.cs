using System;
using System.Windows;
using ChatClient.ViewModels.MainWindowViewModel;

namespace ChatClient.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = (MainWindowViewModel) DataContext;
            viewModel.OpenUserSettingsWindowRequested += OnOpenUserSettingsWindowRequested;
        }

        private static void OnOpenUserSettingsWindowRequested(object sender, EventArgs e)
        {
            var view = new UserSettingsWindow();
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