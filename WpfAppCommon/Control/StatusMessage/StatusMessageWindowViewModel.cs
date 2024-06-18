using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
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
            Message = string.Join("\n", StatusText.Messages);
        }
        // クリアボタンのコマンド
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            // メッセージをクリア
            StatusText.Messages.Clear();
            // メッセージを初期化
            Message = string.Join("\n", StatusText.Messages);
        });

        public SimpleDelegateCommand<Window> CloseCommand => new ((window) => {
            // ウィンドウを閉じる
            window.Close();

        });
    }
}
