using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using ChatClient.Services;
using ChatClient.ViewModels.Properties;
using SharedClasses;

namespace ChatClient.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        private readonly IClientService clientService;
        private bool? isInDesignMode;

        protected ViewModel()
        {
            if (!IsInDesignMode)
            {
                clientService = ServiceManager.GetService<IClientService>();
            }
        }

        public IClientService ClientService
        {
            get { return clientService; }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend or Visual Studio).
        /// </summary>
        protected bool IsInDesignMode
        {
            get
            {
                if (!isInDesignMode.HasValue)
                {
#if DEBUG
#if SILVERLIGHT
_isInDesignMode = DesignerProperties.IsInDesignTool;
#else
                    DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
                    isInDesignMode =
                        (bool) DependencyPropertyDescriptor.FromProperty(prop, typeof (FrameworkElement)).Metadata.DefaultValue;

                    if (!isInDesignMode.Value)
                    {
                        if (Process.GetCurrentProcess().ProcessName.StartsWith(@"devenv"))
                        {
                            isInDesignMode = true;
                        }
                    }
#endif

#else
_isInDesignMode = false;
#endif
                }
                return isInDesignMode.Value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}