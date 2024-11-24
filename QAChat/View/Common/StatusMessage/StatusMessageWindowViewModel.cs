using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Resource;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.View.Common.StatusMessage
{
    public class StatusMessageWindowViewModel : ObservableObject
    {
        private string _message = string.Empty;
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(nameof(Message)); }
        }

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

        public StatusMessageWindowViewModel()
        {
            // メッセージを初期化
            Message = string.Join("\n", StatusText.Messages);
            // メッセージ内のエスケープ済みの改行コードを復元
            Message = Message.Replace("\\n", "\n");

        }
        // クリアボタンのVisible
        public Visibility ClearButtonVisibility { get; set; } = Visibility.Visible;

        // クリアボタンのコマンド
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) =>
        {
            // メッセージをクリア
            StatusText.Messages.Clear();
            // メッセージを初期化
            Message = "";
        });

        public SimpleDelegateCommand<Window> CloseCommand => new((window) =>
        {
            // ウィンドウを閉じる
            window.Close();

        });
    }
}
