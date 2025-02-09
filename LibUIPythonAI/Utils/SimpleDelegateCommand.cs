using System.Windows.Input;

namespace LibUIPythonAI.Utils {
    // Create a class that implements ICommand and accepts a delegate.
    public class SimpleDelegateCommand<T> : ICommand {
        // Specify the keys and mouse actions that invoke the command. 
        public Key GestureKey { get; set; }
        public ModifierKeys GestureModifier { get; set; }
        public MouseAction MouseGesture { get; set; }

        readonly Action<T?> _executeDelegate;

        public SimpleDelegateCommand(Action<T> executeDelegate) {
            _executeDelegate = executeDelegate!;
        }

        public void Execute() {
            Execute(null);
        }

        public void Execute(object? parameter) {
            // System.Windows.MessageBox.Show("Execute");
            _executeDelegate((T?)parameter);
        }

        public bool CanExecute(object? parameter) { return true; }
        public event EventHandler? CanExecuteChanged;


        public static SimpleDelegateCommand<object> EmptyCommand => new((parameter) => { });
    }
}
