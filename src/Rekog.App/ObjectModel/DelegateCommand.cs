using System;
using System.Windows.Input;

namespace Rekog.App.ObjectModel
{
    public class DelegateCommand<T> : DelegateCommandBase<T>
    {
        public DelegateCommand(Action<T?> action)
            : base(action, null)
        {
        }

        public DelegateCommand(Action<T?> action, Func<T?, bool> canExecute)
            : base(action, canExecute)
        {
        }

        public bool CanExecute(T? parameter)
        {
            return CanExecuteCore(parameter);
        }

        public void Execute(T? parameter)
        {
            CanExecuteCore(parameter);
        }
    }

    public class DelegateCommand : DelegateCommandBase<object>
    {
        public DelegateCommand(Action action)
            : base(_ => action.Invoke(), null)
        {
        }

        public DelegateCommand(Action action, Func<bool> canExecute)
            : base(_ => action.Invoke(), _ => canExecute.Invoke())
        {
        }

        public bool CanExecute()
        {
            return CanExecuteCore(null);
        }

        public void Execute()
        {
            CanExecuteCore(null);
        }
    }

    // TODO: Use WeakReference
    public abstract class DelegateCommandBase<T> : ICommand
    {
        private readonly Action<T?> _action;
        private readonly Func<T?, bool>? _canExecute;

        protected DelegateCommandBase(Action<T?> action, Func<T?, bool>? canExecute)
        {
            _action = action;
            _canExecute = canExecute;
            CommandManager.RequerySuggested += OnCommandManagerRequerySuggested;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual bool CanExecuteCore(T? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        protected virtual void ExecuteCore(T? parameter)
        {
            _action.Invoke(parameter);
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return CanExecuteCore((T?)parameter);
        }

        void ICommand.Execute(object? parameter)
        {
            ExecuteCore((T?)parameter);
        }

        private void OnCommandManagerRequerySuggested(object? sender, EventArgs args)
        {
            RaiseCanExecuteChanged();
        }

        public event EventHandler? CanExecuteChanged;
    }
}
