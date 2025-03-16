using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;

namespace AIChatExplorer.ViewModel.Settings {
    internal class TestResultUserControlViewModel : ObservableObject {
        public TestResultUserControlViewModel(string logText) {
            LogText = logText;
        }

        public static CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

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
