using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using ChatClient.ViewModels.Properties;

namespace ChatClient.ViewModels
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool? isInDesignMode;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend or Visual Studio).
        /// </summary>
        protected bool IsInDesignModeStatic
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
                        if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.StartsWith(@"devenv"))
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
    }
}