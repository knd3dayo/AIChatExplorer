using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.SettingWindow {
    internal class TestResultWindowViewModel :ObservableObject{
        public TestResultWindowViewModel() {
        }

        private string logText = "";
        public string LogText {
            get {
                return logText;
            }
            set {
                logText = value;
                OnPropertyChanged("LogText");
            }
        }

        // CancelCommand
        public SimpleDelegateCommand CancelCommand => new((parameter) => {
            WpfAppCommon.Properties.Settings.Default.Reload();
            Tools.Info("設定をキャンセルしました");
            // Windowを閉じる
            if (parameter is Window window) {
                window.Close();
            }
        });
    }
}
