using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Model;
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

        private Action<ScriptItem> afterSelect = (scriptItem) => { };

        // IsExecModeがTrueの時は、実行ボタンを表示する。
        public Visibility ExecButtonVisibility {
            get {
                return IsExecMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public void InitializeEdit() {
            IsExecMode = false;
            ScriptItems.Clear();
            foreach (var item in ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()) {
                ScriptItems.Add(item);
            }
            OnPropertyChanged(nameof(ScriptItems));
            OnPropertyChanged(nameof(ExecButtonVisibility));
        }

        public void InitializeExec(Action<ScriptItem> afterSelect) {
            IsExecMode = true;
            this.afterSelect = afterSelect;

            ScriptItems.Clear();
            foreach (var item in ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()) {
                ScriptItems.Add(item);
            }
            OnPropertyChanged(nameof(ScriptItems));
            OnPropertyChanged(nameof(ExecButtonVisibility));
        }

        // Scriptを新規作成するときの処理
        public static SimpleDelegateCommand CreateScriptItemCommand => new(PythonCommands.CreateScriptCommandExecute);

        // Scriptを編集するときの処理
        public SimpleDelegateCommand EditScriptItemCommand => new((parameter) => {
            if (SelectedScriptItem == null) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            PythonCommands.EditScriptItemCommandExecute(SelectedScriptItem);
        });
        // Scriptを削除したときの処理
        public SimpleDelegateCommand DeleteScriptCommand => new((parameter) => {
            if (SelectedScriptItem == null) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            ScriptItem.DeleteScriptItem(SelectedScriptItem);
            ScriptItems.Remove(SelectedScriptItem);
            OnPropertyChanged(nameof(ScriptItems));
        });

        // 選択ボタンを押したときの処理
        public SimpleDelegateCommand SelectScriptItemCommand => new((parameter) => {
            if (_selectedScriptItem == null) {
                Tools.Error("スクリプトを選択してください");
                return;
            }
            // Actionを実行
            afterSelect(_selectedScriptItem);
            // ウィンドウを閉じる
            if (parameter is ListPythonScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();

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
