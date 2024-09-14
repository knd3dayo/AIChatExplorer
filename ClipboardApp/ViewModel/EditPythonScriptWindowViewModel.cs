using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using ClipboardApp.View.PythonScriptView;
using ClipboardApp.Model;

namespace ClipboardApp.ViewModel
{
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
                LogWrapper.Error(StringResources.EnterDescription);
                return;

            }
            // Scriptの保存
            ScriptItem.SaveScriptItem(ScriptItem);

            // ウィンドウを閉じる
            editScriptWindow.Close();
        });

    }
}
