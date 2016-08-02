using System;
using System.Windows;
using ChatClient.ViewModels.UserSettingsViewModel;
using Microsoft.Win32;
using SharedClasses;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for UserSettingsWindow.xaml
    /// </summary>
    public partial class UserSettingsWindow
    {
        public UserSettingsWindow(IServiceRegistry serviceRegistry)
        {
            var userSettingsViewModel = new UserSettingsViewModel(serviceRegistry);
            userSettingsViewModel.CloseUserSettingsWindowRequest += OnCloseUserSettingsWindowRequest;

            InitializeComponent();
            DataContext = userSettingsViewModel;
        }

        private void OnCloseUserSettingsWindowRequest(object sender, EventArgs e)
        {
            Close();
        }

        private void OnBrowseImageButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog { Filter = "Image Files (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg" };

            fileDialog.ShowDialog();

            string fileLocation = fileDialog.FileName;

            var viewModel = (UserSettingsViewModel) DataContext;

            viewModel.ApplyAvatarToPreviewBox(fileLocation);
        }
    }
}