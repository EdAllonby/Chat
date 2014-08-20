using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using ChatClient.Services;
using SharedClasses;

namespace ChatClient.Views
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
#if DEBUG
            AllocConsole();
#endif
            Thread.CurrentThread.Name = "Main Thread";

            RegisterServices();

            base.OnStartup(e);
        }

        private static void RegisterServices()
        {
            ServiceManager.RegisterService<IClientService>(new ClientService());
        }

        protected override void OnExit(ExitEventArgs e)
        {
#if DEBUG
            FreeConsole();
#endif
            base.OnExit(e);
        }
    }
}