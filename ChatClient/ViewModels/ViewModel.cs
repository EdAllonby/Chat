using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ChatClient.Commands;
using ChatClient.Properties;

namespace ChatClient.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        // I still can't find a way of sharing this Client across. Made it static in the ViewModel for the time being.
        // I might have to investigate IoC to get the viewModels to pass this client about.
        protected static readonly Client Client = new Client();
        public event PropertyChangedEventHandler PropertyChanged;

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