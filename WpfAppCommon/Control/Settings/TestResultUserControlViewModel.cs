using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Control.Settings {
    internal class TestResultUserControlViewModel :ObservableObject{
        public TestResultUserControlViewModel() {
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
        public SimpleDelegateCommand<Window> CancelCommand => new((window) => {
            WpfAppCommon.Properties.Settings.Default.Reload();
            Tools.Info("設定をキャンセルしました");
            // Windowを閉じる
            window.Close();
        });
    }
}
