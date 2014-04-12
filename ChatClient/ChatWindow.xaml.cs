﻿using System;
using System.Collections.Generic;
using System.Windows;
using SharedClasses.Domain;

namespace ChatClient
{
    /// <summary>
    ///     Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow
    {
        private readonly Client client;

        public ChatWindow()
        {
            client = Client.GetInstance();
            client.OnNewContributionNotification += UpdateChatTextBlock;
            client.OnNewUser += UpdateUserList;
            InitializeComponent();
            TitleLabel.Content = "Hello " + client.UserName;
        }

        private void UpdateUserList(IList<User> users, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ConnectedUsersListBox.Items.Clear();
                foreach (User user in users)
                {
                    ConnectedUsersListBox.Items.Add(user.UserName);
                }
            });
        }

        private void UpdateChatTextBlock(string contribution, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => { ChatTextBlock.Items.Add(contribution); });
        }

        private void SendNotificationRequestButton_Click(object sender, RoutedEventArgs e)
        {
            client.SendContributionRequestMessage(ContributionRequestTextBox.Text);
            ContributionRequestTextBox.Text = "Type your message...";
        }

        private void EnterTextBox(object sender, RoutedEventArgs e)
        {
            if (ContributionRequestTextBox.Text == "Type your message...")
            {
                ContributionRequestTextBox.Text = string.Empty;
            }
        }
    }
}