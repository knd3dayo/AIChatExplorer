using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfAppCommon.Model {
    public class StatusText : ObservableObject {

        private static StatusText? _instance;
        public static StatusText Instance {
            get {
                if (_instance == null) {
                    _instance = new StatusText();
                }
                return _instance;
            }
        }

        private const string _DefaultText = "Ready";
        private const string _InProgressText = "In progress...";

        private CancellationTokenSource? _tokenSource;

        public string ReadyText { get; set; } = _DefaultText;

        public string InProgressText { get; set; } = _InProgressText;

        public bool IsInProgress { get; private set; } = false;

        public void UpdateInProgress(bool value, string inProgressText = "") {
            IsInProgress = value;
            if (value) {
                InProgressText = inProgressText;
            } else {
                Ready();
            }
        }

        public static List<string> Messages { get; } = new List<string>();

        private string _text = _DefaultText;
        public string Text {
            get {
                // _textの値が100文字以上の場合は50文字目 + "..."にする。.また、改行コードを削除する
                string result_text;
                if (_text.Length > 100) {
                    result_text = _text.Substring(0, 100) + "...";
                } else {
                    result_text = _text;
                }
                // 改行コードを削除する
                result_text = result_text.Replace("\r", "").Replace("\n", "");

                return result_text;
            }
            set {
                _text = value;
                // InitText以外の場合はメッセージを追加
                if (value != ReadyText) {
                    Messages.Add($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} {value}");
                }
                OnPropertyChanged(nameof(Text));
                // _tokenSourceがnullの場合は初期化
                if (_tokenSource != null) {
                    //すでに_tokenSourceが存在する場合はキャンセル
                    _tokenSource.Cancel();
                }
                _tokenSource = new CancellationTokenSource();
                //ThreadPoolに新規処理を追加
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClearText), _tokenSource.Token);

            }
        }

        private void ClearText(object? obj) {
            if (obj == null) {
                return;
            }
            CancellationToken token = (CancellationToken)obj;
            for (int i = 0; i < 50; i++) {
                if (token.IsCancellationRequested) {
                    return;
                }
                // 100ms待機
                Thread.Sleep(100);
            }
            if (token.IsCancellationRequested) {
                return;
            }
            if (IsInProgress) {
                Text = InProgressText;
            } else {
                Ready();
            }
        }
        public void Init() {
            ReadyText = _DefaultText;
            Text = ReadyText;
            OnPropertyChanged(nameof(Text));
        }
        public void Ready() {
            Text = ReadyText;
            OnPropertyChanged(nameof(Text));
        }
    }

}
