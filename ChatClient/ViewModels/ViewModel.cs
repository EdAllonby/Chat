using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ChatClient.Annotations;
using ChatClient.Commands;

namespace ChatClient.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // I still can't find a way of sharing this Client across. Made it static in the ViewModel for the time being.
        // I might have to investigate IoC to get the viewModels to pass this client about.
        protected static Client Client = new Client();

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Close Command

        public static ICommand CloseWindow
        {
            get { return new RelayCommand(() => Application.Current.Shutdown()); }
        }

        #endregion
    }
}