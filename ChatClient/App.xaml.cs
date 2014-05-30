using System.Threading;
using System.Windows;
using ChatClient.Services;
using SharedClasses;

namespace ChatClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // First thing is to name the main thread, then continue with the normal startup procedure.
            Thread mainThread = Thread.CurrentThread;
            mainThread.Name = "Main Thread";
            
            RegisterServices();

            base.OnStartup(e);
        }

        private void RegisterServices()
        {
            ServiceManager.RegisterService<IClientService>(new ClientService());
        }
    }
}