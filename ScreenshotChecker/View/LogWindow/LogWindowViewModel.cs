using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ScreenshotChecker.View.LogWindow {
    internal class LogWindowViewModel :MyWindowViewModel{
        public LogWindowViewModel() {
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

        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            // Windowを閉じる
            if (parameter is Window window) {
                window.Close();
            }
        });

    }
}
