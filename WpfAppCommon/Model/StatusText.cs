using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfAppCommon.Model {
    public class StatusText : ObservableObject {

        // StatusTextDictionaryのキー
        public static string GetStatusTextKey(Window window) {
            Type type = window.GetType();
            string? key = type.FullName;
            if (key == null) {
                key = type.Name;
            }
            return key;
        }

        public static Dictionary<string, StatusText> StatusTextDictionary { get; } = [];

        public static StatusText GetStatusText(Window window) {
            string key = GetStatusTextKey(window);
            if (StatusTextDictionary.ContainsKey(key)) {
                return StatusTextDictionary[key];
            } else {
                StatusText statusText = new();
                StatusTextDictionary.Add(key, statusText);
                return statusText;
            }
        }

        public static string DefaultText { get; } = "Ready";

        private string _text = DefaultText;
        private CancellationTokenSource? _tokenSource;

        public string ReadyText { get; set; } = DefaultText;

        public static List<string> Messages { get; } = new List<string>();

        public string Text {
            get {
                return _text;
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
            Ready();
        }
        public void Init() {
            ReadyText = DefaultText;
            Ready();
        }
        public void Ready() {
            Text = ReadyText;
            OnPropertyChanged(nameof(Text));
        }
        private void Dispose() {
            if (_tokenSource != null) {
                _tokenSource.Cancel();
            }
        }
        public static void DisposeAll() {
            foreach (StatusText statusText in StatusTextDictionary.Values) {
                statusText.Dispose();
            }
        }
    }

}
