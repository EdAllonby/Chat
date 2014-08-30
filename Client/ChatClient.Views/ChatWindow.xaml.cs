using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ChatClient.ViewModels.ChatWindowViewModel;
using ChatClient.ViewModels.UserSettingsViewModel;
using Microsoft.Win32;
using SharedClasses;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        private readonly IServiceRegistry serviceRegistry;

        public ChatWindow(IServiceRegistry serviceRegistry, ChatWindowViewModel viewModel)
        {
            this.serviceRegistry = serviceRegistry;
            viewModel.OpenUserSettingsWindowRequested += OnOpenUserSettingsWindowRequested;
            InitializeComponent();
            DataContext = viewModel;

            viewModel.InitialiseChat();
        }

        private void OnOpenUserSettingsWindowRequested(object sender, EventArgs e)
        {
            var view = new UserSettingsWindow(serviceRegistry);
            view.ShowDialog();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            // Cannot directly bind a command to a closing event, so need to call the command in code behind.
            var viewModel = DataContext as ChatWindowViewModel;
            if (viewModel != null)
            {
                viewModel.Closing.Execute(null);
            }
        }

        private void OnMessageTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            MessageTextBox.ScrollToEnd();
        }

        private void FileShowTextBox_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.All;
            e.Handled = true;
        }

        private void OnBrowseImageButtonClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog { Filter = "Image Files (*.bmp, *.png, *.jpg)|*.bmp;*.png;*.jpg" };

            fileDialog.ShowDialog();

            string fileLocation = fileDialog.FileName;

            var viewModel = (ChatWindowViewModel)DataContext;

            viewModel.SendImageContribution(fileLocation);
        }
    }
}