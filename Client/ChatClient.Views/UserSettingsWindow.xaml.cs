using System;
using ChatClient.ViewModels.UserSettingsViewModel;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for UserSettingsWindow.xaml
    /// </summary>
    public partial class UserSettingsWindow
    {
        public UserSettingsWindow()
        {
            InitializeComponent();
            var userSettingsViewModel = (UserSettingsViewModel) DataContext;
            userSettingsViewModel.CloseUserSettingsWindowRequest += OnCloseUserSettingsWindowRequest;
        }

        private void OnCloseUserSettingsWindowRequest(object sender, EventArgs e)
        {
            Close();
        }
    }
}