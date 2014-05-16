using System.ComponentModel;
using System.Runtime.CompilerServices;
using ChatClient.Annotations;

namespace ChatClient.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        // I still can't find a way of sharing this Client across. Made it static in the ViewModel for the time being.
        // I might have to investigate IoC to get the viewModels to pass this client about.
        protected static readonly Client Client = new Client();

        /// <summary>
        /// Holds the states of conversation windows for the Client
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