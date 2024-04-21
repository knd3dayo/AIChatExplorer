using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace QAChat.View.LogWindow {
    internal class LogWindowViewModel :ObservableObject{
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
    }
}
