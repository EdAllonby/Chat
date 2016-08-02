using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ChatClient.ViewModels.ChatWindowViewModel;
using Microsoft.Win32;
using SharedClasses;
using SharedClasses.Domain;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        private readonly IServiceRegistry serviceRegistry;
        private readonly ChatWindowViewModel viewModel;
        private bool hadText;

        public ChatWindow(IServiceRegistry serviceRegistry, Conversation conversation)
        {
            this.serviceRegistry = serviceRegistry;
            var chatWindowViewModel = new ChatWindowViewModel(conversation, serviceRegistry);
            viewModel = chatWindowViewModel;
            chatWindowViewModel.OpenUserSettingsWindowRequested += OnOpenUserSettingsWindowRequested;
            InitializeComponent();
            DataContext = chatWindowViewModel;

            chatWindowViewModel.InitialiseChat();
        }

        private void OnOpenUserSettingsWindowRequested(object sender, EventArgs e)
        {
            var view = new UserSettingsWindow(serviceRegistry);
            view.ShowDialog();
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            // Cannot directly bind a command to a closing event, so need to call the command in code behind.
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

            viewModel.SendImageContribution(fileLocation);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!hadText)
            {
                if (!string.IsNullOrEmpty(ChatTextBox.Text))
                {
                    hadText = true;
                    viewModel.SendUserTypingRequest(true);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ChatTextBox.Text))
                {
                    hadText = false;
                    viewModel.SendUserTypingRequest(false);
                }
            }
        }
    }
}