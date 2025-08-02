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

        private const int ClearTextLoopCount = 25;
        private const int ClearTextWaitMs = 200;

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
                // _textの値が100文字以上の場合は100文字目 + "..."にする。また、改行コードを削除する
                string result_text;
                if (_text.Length > 100) {
                    result_text = _text.Substring(0, 100) + "...";
                } else {
                    result_text = _text;
                }
                result_text = result_text.Replace("\n", "");
                return result_text;
            }
            set {
                _text = value;
                OnPropertyChanged(nameof(Text));

                if (_tokenSource != null) {
                    _tokenSource.Cancel();
                    _tokenSource.Dispose();
                }
                _tokenSource = new CancellationTokenSource();
                // Task.Runのみで十分
                Task.Run(() => ClearText(_tokenSource.Token));
            }
        }

        private void ClearText(CancellationToken token) {
            for (int i = 0; i < ClearTextLoopCount; i++) {
                if (token.IsCancellationRequested) {
                    return;
                }
                Thread.Sleep(ClearTextWaitMs);
            }
            if (token.IsCancellationRequested) {
                return;
            }
            MainUITask.Run(() => {
                if (IsInProgress) {
                    Text = InProgressText;
                } else {
                    // TextプロパティのsetterでOnPropertyChangedが呼ばれるのでReady()のOnPropertyChangedは不要
                    Text = ReadyText;
                }
            });
        }
        public void Init() {
            ReadyText = _DefaultText;
            Text = ReadyText;
        }
        public void Ready() {
            Text = ReadyText;
        }
    }

}
