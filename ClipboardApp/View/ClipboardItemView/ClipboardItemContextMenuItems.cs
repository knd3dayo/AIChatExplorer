using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Views.ClipboardItemView {
    public class ClipboardItemFolderContextMenuItems : ObservableCollection<ClipboardAppMenuItem> {

        private ClipboardItemViewModel _itemViewModel;
        private MainWindowViewModel _mainWindowViewModel {
            get {
                return _itemViewModel.FolderViewModel.MainWindowViewModel;
            }
        }

        public ClipboardItemFolderContextMenuItems(ClipboardItemViewModel itemViewModel) {
            _itemViewModel = itemViewModel;
            InitContextMenu();
        }

        private void InitContextMenu() {
            // コンテキストメニューの初期化
            Add(new ClipboardAppMenuItem("開く", _mainWindowViewModel.OpenSelectedItemCommand, "Ctrl+O"));
            Add(new ClipboardAppMenuItem("テキストファイルとして開く", _mainWindowViewModel.OpenSelectedItemAsFileCommand, "Ctrl+Shit+O"));
            Add(new ClipboardAppMenuItem("ピン留め", _mainWindowViewModel.ChangePinCommand));

            Add(new ClipboardAppMenuItem("コピー", _mainWindowViewModel.CopyItemCommand, "Ctrl+C"));
            Add(new ClipboardAppMenuItem("削除", _mainWindowViewModel.DeleteSelectedItemCommand, "Delete"));

            // サブメニュー設定
            ClipboardAppMenuItem utilityMenuItems = new("ツール", SimpleDelegateCommand.EmptyCommand);
            // タイプがFileの場合
            if (_itemViewModel.ContentType == ClipboardContentTypes.Files) {
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("フォルダを開く", _mainWindowViewModel.OpenFolderCommand));
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("ファイルを開く", _mainWindowViewModel.OpenFileCommand));
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("一時フォルダでファイルを開く", _mainWindowViewModel.OpenFileInTempFolderCommand));
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("ファイルからテキストを抽出", ClipboardItemViewModel.ExtractTextCommand));
            }
            // タイプがImageの場合
            if (_itemViewModel.ContentType == ClipboardContentTypes.Image) {
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("画像からテキストを抽出",
                    new SimpleDelegateCommand((parameter) => {
                        ClipboardItemViewModel.MenuItemExtractTextFromImageCommandExecute(_mainWindowViewModel.SelectedItem);
                    })));
            }
            // 全タイプ共通
            if (ClipboardAppConfig.UseSpacy && ClipboardAppConfig.PythonExecute != 0) {
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データをマスキング", ClipboardItemViewModel.MaskDataCommand));
            }

            Add(utilityMenuItems);

            // AI関連のメニュー
            ClipboardAppMenuItem aiUtilityMenuItems = new("OpenAI", SimpleDelegateCommand.EmptyCommand);

            aiUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("OpenAIチャット",
                new SimpleDelegateCommand((parameter) => {
                    ClipboardItemViewModel.OpenOpenAIChatWindowExecute(_mainWindowViewModel.SelectedItem);
                })));

            aiUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("プロンプトテンプレートを実行",
                    new SimpleDelegateCommand((parameter) => {
                        ClipboardItemViewModel.OpenAIChatCommandExecute(_mainWindowViewModel.SelectedItem);
                    })));

            // UseOpenAI=Trueの場合は、便利機能にAI関連のメニューを追加
            if (ClipboardAppConfig.UseOpenAI && ClipboardAppConfig.PythonExecute != 0) {
                Add(aiUtilityMenuItems);
            }

            // ユーザー定義のPythonスクリプトをメニュー
            ClipboardAppMenuItem userDefinedPythonScriptsMenu
                = new("Pythonスクリプトを実行", new SimpleDelegateCommand((parameter) => {
                    if (_mainWindowViewModel.SelectedItem == null) {
                        Tools.Error("スクリプトを選択してください");
                        return;
                    }
                    PythonCommands.OpenListPythonScriptWindowExecCommandExecute((scriptItem) => {
                        ClipboardItemViewModel.MenuItemRunPythonScriptCommandExecute(scriptItem, _mainWindowViewModel.SelectedItem);
                    }
                    );
                })
                );

            // PythonExecuteが0以外の場合は、便利機能にユーザー定義のPythonスクリプトを追加
            if (ClipboardAppConfig.PythonExecute != 0) {
                Add(userDefinedPythonScriptsMenu);
            }

        }
    }
}
