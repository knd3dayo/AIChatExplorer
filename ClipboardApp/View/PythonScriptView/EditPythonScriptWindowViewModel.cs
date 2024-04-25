using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.PythonScriptView {
    class EditPythonScriptWindowViewModel : ObservableObject {
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
                Tools.Error("説明を入力してください");
                return;

            }
            // Scriptの保存
            ScriptItem.SaveScriptItem(ScriptItem);

            // ウィンドウを閉じる
            if (parameter is EditPythonScriptWindow editScriptWindow) {
                editScriptWindow.Close();
            }
        }
        // キャンセルボタンのコマンド
        public SimpleDelegateCommand CancelButtonCommand => new SimpleDelegateCommand(CancelButtonCommandExecute);
        private void CancelButtonCommandExecute(object parameter) {
            // ウィンドウを閉じる
            if (parameter is EditPythonScriptWindow editScriptWindow) {
                editScriptWindow.Close();
            }
        }

    }
}
