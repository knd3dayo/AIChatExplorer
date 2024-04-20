using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Model;
using ClipboardApp.PythonIF;
using ClipboardApp.Utils;

namespace ClipboardApp.View.ScriptView {
    class EditScriptWindowViewModel : ObservableObject {
        private ScriptItem? scriptItem;
        public ScriptItem? ScriptItem {
            get {
                return scriptItem;
            }
            set {
                scriptItem = value;
                OnPropertyChanged("ScriptItem");
            }
        }

        // OKボタンのコマンド
        public SimpleDelegateCommand OKButtonCommand => new SimpleDelegateCommand(OKButtonCommandExecute);
        private void OKButtonCommandExecute(object parameter) {
            // ScriptItemのチェック
            if (ScriptItem == null) {
                return;
            }
            //　descriptionのチェック
            if (string.IsNullOrEmpty(ScriptItem.Description)) {
                Tools.ShowMessage("説明を入力してください");
                return;

            }
            // Scriptの保存
            PythonExecutor.SaveScriptItem(ScriptItem);

            // ウィンドウを閉じる
            if (parameter is EditScriptWindow editScriptWindow) {
                editScriptWindow.Close();
            }
        }
        // キャンセルボタンのコマンド
        public SimpleDelegateCommand CancelButtonCommand => new SimpleDelegateCommand(CancelButtonCommandExecute);
        private void CancelButtonCommandExecute(object parameter) {
            // ウィンドウを閉じる
            if (parameter is EditScriptWindow editScriptWindow) {
                editScriptWindow.Close();
            }
        }

    }
}
