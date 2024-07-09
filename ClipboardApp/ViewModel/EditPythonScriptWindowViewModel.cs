using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using ClipboardApp.View.PythonScriptView;

namespace ClipboardApp.ViewModel {
    class EditPythonScriptWindowViewModel : MyWindowViewModel {
        private ScriptItem? scriptItem;
        public ScriptItem? ScriptItem {
            get {
                return scriptItem;
            }
            set {
                scriptItem = value;
                OnPropertyChanged(nameof(ScriptItem));
            }
        }

        // OKボタンのコマンド
        public SimpleDelegateCommand<EditPythonScriptWindow> OKButtonCommand => new((editScriptWindow) => {
            // ScriptItemのチェック
            if (ScriptItem == null) {
                return;
            }
            //　descriptionのチェック
            if (string.IsNullOrEmpty(ScriptItem.Description)) {
                LogWrapper.Error("説明を入力してください");
                return;

            }
            // Scriptの保存
            ScriptItem.SaveScriptItem(ScriptItem);

            // ウィンドウを閉じる
            editScriptWindow.Close();
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<EditPythonScriptWindow> CancelButtonCommand => new((editScriptWindow) => {
            // ウィンドウを閉じる
            editScriptWindow.Close();
        });

    }
}
