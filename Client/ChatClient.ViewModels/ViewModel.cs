using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using ChatClient.ViewModels.Properties;
using SharedClasses;

namespace ChatClient.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged
    {
        private bool? isInDesignMode;

        protected ViewModel(IServiceRegistry serviceRegistry)
        {
            if (!IsInDesignMode)
            {
                ServiceRegistry = serviceRegistry;
            }
        }

        protected IServiceRegistry ServiceRegistry { get; }

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
isInDesignMode = DesignerProperties.IsInDesignTool;
#else
                    DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
                    isInDesignMode =
                        (bool) DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

                    if (!isInDesignMode.Value)
                    {
                        if (Process.GetCurrentProcess().ProcessName.StartsWith(@"devenv"))
                        {
                            isInDesignMode = true;
                        }
                    }
#endif

#else
isInDesignMode = false;
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