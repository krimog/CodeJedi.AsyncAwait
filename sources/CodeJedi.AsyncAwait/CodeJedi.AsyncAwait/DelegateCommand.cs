using System;
using System.Windows.Input;

namespace CodeJedi.AsyncAwait
{
    public class DelegateCommand : ICommand
    {
        public bool IsExecuting { get; private set; }
        private Action ExecuteDelegate { get; }
        private Func<bool> CanExecuteDelegate { get; }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action executeDelegate)
            : this(executeDelegate, null)
        { }

        public DelegateCommand(Action executeDelegate, Func<bool> canExecuteDelegate)
        {
            ExecuteDelegate = executeDelegate;
            CanExecuteDelegate = canExecuteDelegate;
        }

        public bool CanExecute(object parameter)
        {
            return !IsExecuting && (CanExecuteDelegate?.Invoke() ?? true);
        }

        public void Execute(object parameter)
        {
            try
            {
                IsExecuting = true;
                CommandManager.InvalidateRequerySuggested();
                ExecuteDelegate();
            }
            finally
            {
                IsExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
