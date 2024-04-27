using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.PythonScriptView {
    public class ListPythonScriptWindowViewModel : ObservableObject {
        public static ObservableCollection<ScriptItem> ScriptItems { get; } = ScriptItem.ScriptItems;

        private ScriptItem? _selectedScriptItem;

        public ScriptItem? SelectedScriptItem {
            get {
                return _selectedScriptItem;
            }
            set {
                _selectedScriptItem = value;
                OnPropertyChanged(nameof(ScriptItem));
            }
        }

        private bool _isExecMode = false;
        public bool IsExecMode {
            get => _isExecMode;
            set {
                _isExecMode = value;
                OnPropertyChanged(nameof(IsExecMode));
            }
        }
        public string Title {
            get {
                return IsExecMode ? "Pythonスクリプトを選択" : "Pythonスクリプト一覧";
            }
        }
        public string SelectButtonText {
            get {
                return IsExecMode ? "実行" : "選択";
            }
        }
        private ClipboardItemViewModel? _itemViewModel;


        public void InitializeEdit() {
            IsExecMode = false;
            ScriptItems.Clear();
            foreach (var item in ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()) {
                ScriptItems.Add(item);
            }
            OnPropertyChanged(nameof(ScriptItems));
        }

        public void InitializeExec(ClipboardItemViewModel itemViewModel) {
            IsExecMode = true;
            ScriptItems.Clear();
            foreach (var item in ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()) {
                ScriptItems.Add(item);
            }
            OnPropertyChanged(nameof(ScriptItems));
        }

        // Scriptを新規作成するときの処理
        public static SimpleDelegateCommand CreateScriptItemCommand => new(PythonCommands.CreateScriptCommandExecute);

        // Scriptを編集するときの処理
        public  SimpleDelegateCommand EditScriptItemCommand => new((parameter) => {
            if (SelectedScriptItem == null) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            PythonCommands.EditScriptItemCommandExecute(SelectedScriptItem);
        });
        // Scriptを削除したときの処理
        public static SimpleDelegateCommand DeleteScriptCommand => new((parameter) => {
            if (parameter is ScriptItem scriptItem) {
                ScriptItem.DeleteScriptItem(scriptItem);
                ScriptItems.Remove(scriptItem);
            }
        });

        // 選択ボタンを押したときの処理
        public SimpleDelegateCommand SelectScriptItemCommand => new((parameter) => {
            if (parameter is not ScriptItem scriptItem) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            if (IsExecMode) {
                if (_itemViewModel is not null) {
                    // _itemViewModelをJSON化する。
                    string json = ClipboardItem.ToJson(_itemViewModel.ClipboardItem);
                    string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, json);
                    // jsonを復元する。
                    ClipboardItem? clipboardItem = ClipboardItem.FromJson(result, Tools.DefaultAction);
                    if (clipboardItem is not null) {
                        clipboardItem.CopyTo(_itemViewModel.ClipboardItem);
                    }

                }

            } else {

                // EditScriptWindowを開く
                EditPythonScriptWindow editScriptWindow = new();
                // EditScriptWindowのViewModelを取得
                EditPythonScriptWindowViewModel editScriptWindowViewModel = (EditPythonScriptWindowViewModel)editScriptWindow.DataContext;
                editScriptWindowViewModel.ScriptItem = scriptItem;

                editScriptWindow.ShowDialog();
            }

        });
        // キャンセルボタンを押したときの処理
        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            // ウィンドウを閉じる
            if (parameter is ListPythonScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();
            }

        });

    }
}
