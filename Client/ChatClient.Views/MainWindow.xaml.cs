using ChatClient.ViewMediator;
using ChatClient.ViewModels.MainWindowViewModel;

namespace ChatClient.Views
{
    public partial class MainWindow
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            Mediator.Instance.Register(ViewName.UserSettingsWindow, ShowUserSettingsWindow);

        }

        private static void ShowUserSettingsWindow(object obj)
        {
            var view = new UserSettingsWindow();
            view.ShowDialog();
        }
    }
}