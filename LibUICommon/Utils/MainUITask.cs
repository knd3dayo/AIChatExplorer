using System.Windows;

namespace WpfAppCommon.Utils {
    public  class MainUITask {

        public static void Run(Action action) {
            Application.Current.Dispatcher.Invoke(action);
        }

    }
}
