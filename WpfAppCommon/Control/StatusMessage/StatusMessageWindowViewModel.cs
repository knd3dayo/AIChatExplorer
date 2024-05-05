using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace WpfCommonApp.Control.StatusMessage {
    public class StatusMessageWindowViewModel : ObservableObject{
        private string _message = string.Empty;
        public string Message {
            get { return _message; }
            set { _message = value; OnPropertyChanged(nameof(Message)); }
        }


        public void Initialize() {
            // メッセージを初期化
            Message = string.Join("\n", Tools.StatusText.Messages);
        }

        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            // ウィンドウを閉じる
            if (parameter is not System.Windows.Window window) {
                return;
            }
            window.Close();

        });
    }
}
