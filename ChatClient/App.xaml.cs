using System.Windows;

namespace ChatClient
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        ///     Method used to get the command line arguments of the project.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            LoginWindow.CommandLineArguments = e.Args;
            base.OnStartup(e);
        }
    }
}