using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Utils;

namespace LibUIPythonAI.ViewModel.Common {
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
                InProgressText = "";
            }
        }

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
                result_text = result_text.Replace("\n", "");

                return result_text;
            }
            set {
                _text = value;
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
            Task.Run(() => {
                for (int i = 0; i < 25; i++) {
                    if (token.IsCancellationRequested) {
                        return;
                    }
                    // 200ms待機
                    Thread.Sleep(200);
                }
                if (token.IsCancellationRequested) {
                    return;
                }
                MainUITask.Run(() => {
                    if (IsInProgress) {
                        Text = InProgressText;
                    } else {
                        Ready();
                    }
                });
            });
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
