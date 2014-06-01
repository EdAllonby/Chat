using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ChatClient.ViewModels.Commands
{
    internal class RelayCommand : ICommand
    {
        #region Members

        private readonly Func<Boolean> canExecute;
        private readonly Action execute;

        #endregion

        #region Constructors

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<Boolean> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (canExecute != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        [DebuggerStepThrough]
        public Boolean CanExecute(Object parameter)
        {
            return canExecute == null || canExecute();
        }

        public void Execute(Object parameter)
        {
            execute();
        }

        #endregion
    }
}