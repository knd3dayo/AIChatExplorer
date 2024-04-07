using System.Windows.Input;

namespace WpfApp1.Utils
{
    // Create a class that implements ICommand and accepts a delegate.
    public class SimpleDelegateCommand : ICommand
    {
        // Specify the keys and mouse actions that invoke the command. 
        public Key GestureKey { get; set; }
        public ModifierKeys GestureModifier { get; set; }
        public MouseAction MouseGesture { get; set; }

        Action<object?> _executeDelegate;

        public SimpleDelegateCommand(Action<object> executeDelegate)
        {
            _executeDelegate = executeDelegate!;
        }

        public void Execute(object? parameter)
        {
            // System.Windows.MessageBox.Show("Execute");
            _executeDelegate(parameter);
        }

        public bool CanExecute(object? parameter) { return true; }
        public event EventHandler? CanExecuteChanged;
    }
}
