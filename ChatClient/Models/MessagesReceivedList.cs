using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ChatClient.Models
{
    public class MessagesReceivedList : INotifyPropertyChanged
    {
        private readonly Client client = Client.GetInstance();

        public MessagesReceivedList()
        {
            client.OnNewContributionNotification += client_OnNewContributionNotification;
            Messages = new ObservableCollection<string>();
        }

        public ObservableCollection<String> Messages { get; set; }

        private void client_OnNewContributionNotification(string contribution, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => Messages.Add(contribution));
            OnPropertyChanged();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}