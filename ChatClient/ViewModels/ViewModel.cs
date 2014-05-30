using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChatClient.Annotations;

namespace ChatClient.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Holds the states of conversation windows for the ClientService
        /// </summary>
        protected static readonly ConversationWindowsStatusCollection ConversationWindowsStatusCollection = new ConversationWindowsStatusCollection();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}