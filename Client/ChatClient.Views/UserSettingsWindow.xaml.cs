using System;
using ChatClient.ViewModels.UserSettingsViewModel;
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
    }
}