using System.Windows;
using NLog;
using WpfAppCommon.Model;

namespace LibUIPythonAI.Utils {
    public class CustomLogger : Logger {

        public static Window ActiveWindow { get; set; } = Application.Current.MainWindow;

        private static StatusText StatusText {
            get {
                return StatusText.Instance;
            }
        }

        public new void Debug(string message) {
            // 親クラスのメソッドを呼び出す
            base.Debug(message);
            Task.Run(() => {
                StatusText.Messages.Add(message);
            });
        }

        public new void Info(string message) {
            // 親クラスのメソッドを呼び出す
            base.Info(message);
            MainUITask.Run(() => {
                StatusText.Text = message;
            });
        }
        public new void Warn(string message) {
            MainUITask.Run(() => {
                base.Warn(message);
                StatusText.Text = message;
                // 開発中はメッセージボックスを表示する
                MessageBox.Show(ActiveWindow, message);
            });
        }

        public new void Error(string message) {
            MainUITask.Run(() => {
                base.Error(message);
                StatusText.Text = message;
                MessageBox.Show(ActiveWindow, message);
            });
        }

    }
}
