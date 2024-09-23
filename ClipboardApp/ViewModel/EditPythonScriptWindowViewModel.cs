using ClipboardApp.Model;
using ClipboardApp.View.PythonScriptView;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
    class EditPythonScriptWindowViewModel : ClipboardAppViewModelBase {

        public EditPythonScriptWindowViewModel(ScriptItem scriptItem) {
            _scriptItem = scriptItem;
        }

        private ScriptItem _scriptItem;
        public ScriptItem ScriptItem {
            get {
                return _scriptItem;
            }
            set {
                _scriptItem = value;
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
