using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Settings {
    internal class TestResultUserControlViewModel :ObservableObject{
        public TestResultUserControlViewModel(string logText) {
            LogText = logText;
        }

        private string logText = "";
        public string LogText {
            get {
                return logText;
            }
            set {
                logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }

        // CancelCommand
        public SimpleDelegateCommand<Window> CancelCommand => new((window) => {
            Properties.Settings.Default.Reload();
            LogWrapper.Info(CommonStringResources.Instance.Canceled);
            // Windowを閉じる
            window.Close();
        });
    }
}
