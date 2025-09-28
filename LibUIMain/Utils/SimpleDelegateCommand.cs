using System.Windows.Input;

namespace LibUIMain.Utils {
    // Create a class that implements ICommand and accepts a delegate.
    public class SimpleDelegateCommand<T> : ICommand {

        #region ICommand implementation
        public void Execute(object? parameter) {
            if (BeforeAction != null) { BeforeAction(); }

            _executeDelegate((T?)parameter);

            if (AfterAction != null) { AfterAction(); }
        }

        public bool CanExecute(object? parameter) { return true; }

        public event EventHandler? CanExecuteChanged;

        #endregion

        private Action? BeforeAction;
        private Action? AfterAction;

        readonly Action<T?> _executeDelegate;

        public SimpleDelegateCommand(Action<T> executeDelegate, Action? beforeAction = null, Action? afterAction = null) {
            _executeDelegate = executeDelegate!;
            this.BeforeAction = beforeAction;
            this.AfterAction = afterAction;
        }

        public void Execute() {
            Execute(null);
        }


        public static SimpleDelegateCommand<object> EmptyCommand => new((parameter) => { }, () => { }, () => { });
    }
}
