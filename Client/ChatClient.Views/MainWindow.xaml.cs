using System;
using ChatClient.ViewModels.MainWindowViewModel;

namespace ChatClient.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            MainWindowViewModel viewModel = (MainWindowViewModel) DataContext;
            viewModel.OpenUserSettingsWindowRequested += OnOpenUserSettingsWindowRequested;
        }

        private static void OnOpenUserSettingsWindowRequested(object sender, EventArgs e)
        {
            var view = new UserSettingsWindow();
            view.ShowDialog();
        }
    }
}