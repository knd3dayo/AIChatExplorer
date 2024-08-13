using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using WpfAppCommon.Model;

namespace WpfAppCommon.Utils {
    public class CustomLogger : Logger{

        public static Window ActiveWindow { get; set; } = Application.Current.MainWindow;

        public static StatusText StatusText {
            get {
                return StatusText.GetStatusText(ActiveWindow);
            }
        }
        public new void Info(string message) {
            // 親クラスのメソッドを呼び出す
            base.Info(message);
            Application.Current.Dispatcher.Invoke(() => {
                if (StatusText != null) {
                    StatusText.Text = message;
                }
            });
        }
        public new void Warn(string message) {
            Application.Current.Dispatcher.Invoke(() => {
                base.Warn(message);
                if (StatusText != null) {
                    StatusText.Text = message;
                }
                // 開発中はメッセージボックスを表示する
                System.Windows.MessageBox.Show(ActiveWindow, message);
            });
        }

        public new void Error(string message) {
            Application.Current.Dispatcher.Invoke(() => {
                base.Error(message);
                if (StatusText != null) {
                    StatusText.Text = message;
                }
                System.Windows.MessageBox.Show(ActiveWindow, message);
            });
        }

    }
}
