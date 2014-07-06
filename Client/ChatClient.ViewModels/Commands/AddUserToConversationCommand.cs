using System;
using System.Windows.Input;

namespace ChatClient.ViewModels.Commands
{
    internal class AddUserToConversationCommand : ICommand
    {
        private readonly ChatWindowViewModel.ChatWindowViewModel viewModel;

        public AddUserToConversationCommand(ChatWindowViewModel.ChatWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return viewModel.CanAddUser(parameter);
        }

        public void Execute(object parameter)
        {
            viewModel.AddUser(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}