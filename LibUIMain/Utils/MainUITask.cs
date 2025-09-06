using System.Windows;

namespace LibUIMain.Utils {
    public class MainUITask {

        public static void Run(Action? action) {
            if (action == null) {
                return;
            }
            Application.Current.Dispatcher.Invoke(action);
        }

    }
}
