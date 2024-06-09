using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Views.ClipboardItemView {
    public class ClipboardItemFolderContextMenuItems : ObservableCollection<ClipboardAppMenuItem> {

        private ClipboardItemViewModel _itemViewModel;

        public ClipboardItemFolderContextMenuItems(ClipboardItemViewModel itemViewModel) {
            _itemViewModel = itemViewModel;
            InitContextMenu();
        }

        private void InitContextMenu() {
            // MainViewModelが存在しない場合は何もしない
            if (MainWindowViewModel.ActiveInstance == null) {
                return;
            }
            // コンテキストメニューの初期化
            Add(new ClipboardAppMenuItem("開く", MainWindowViewModel.ActiveInstance.OpenSelectedItemCommand, "Ctrl+O"));
            Add(new ClipboardAppMenuItem("テキストファイルとして開く", MainWindowViewModel.ActiveInstance.OpenSelectedItemAsFileCommand, "Ctrl+Shit+O"));
            Add(new ClipboardAppMenuItem("ピン留め", ClipboardItemViewModel.ChangePinCommand));

            Add(new ClipboardAppMenuItem("コピー", MainWindowViewModel.ActiveInstance.CopyItemCommand, "Ctrl+C"));
            Add(new ClipboardAppMenuItem("削除", MainWindowViewModel.ActiveInstance.DeleteSelectedItemCommand, "Delete"));

            // サブメニュー設定
            ClipboardAppMenuItem utilityMenuItems = new("ツール", SimpleDelegateCommand<object>.EmptyCommand);
            // タイプがFileの場合
            if (_itemViewModel.ContentType == ClipboardContentTypes.Files) {
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("フォルダを開く", MainWindowViewModel.ActiveInstance.OpenFolderCommand));
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("ファイルを開く", MainWindowViewModel.ActiveInstance.OpenFileCommand));
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("一時フォルダでファイルを開く", MainWindowViewModel.ActiveInstance.OpenFileInTempFolderCommand));
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("ファイルからテキストを抽出", ClipboardItemViewModel.ExtractTextCommand));
            }
            // タイプがImageの場合
            if (_itemViewModel.ContentType == ClipboardContentTypes.Image) {
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("画像からテキストを抽出",
                    new SimpleDelegateCommand<object>((parameter) => {
                        ClipboardItemViewModel.MenuItemExtractTextFromImageCommandExecute(MainWindowViewModel.ActiveInstance.SelectedItem);
                    })));
            }
            // 全タイプ共通
            if (ClipboardAppConfig.UseSpacy && ClipboardAppConfig.PythonExecute != 0) {
                utilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データをマスキング", ClipboardItemViewModel.MaskDataCommand));
            }

            Add(utilityMenuItems);

            // AI関連のメニュー
            ClipboardAppMenuItem aiUtilityMenuItems = new("OpenAI", SimpleDelegateCommand<object>.EmptyCommand);

            aiUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("OpenAIチャット",
                new SimpleDelegateCommand<object>((parameter) => {
                    ClipboardItemViewModel.OpenOpenAIChatWindowExecute(
                        MainWindowViewModel.ActiveInstance.SelectedFolder,
                        MainWindowViewModel.ActiveInstance.SelectedItem);
                })));

            aiUtilityMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("プロンプトテンプレートを実行",
                    new SimpleDelegateCommand<object>((parameter) => {
                        ClipboardItemViewModel.OpenAIChatCommandExecute(MainWindowViewModel.ActiveInstance.SelectedItem);
                    })));

            // UseOpenAI=Trueの場合は、便利機能にAI関連のメニューを追加
            if (ClipboardAppConfig.UseOpenAI && ClipboardAppConfig.PythonExecute != 0) {
                Add(aiUtilityMenuItems);
            }

            // ユーザー定義のPythonスクリプトをメニュー
            ClipboardAppMenuItem userDefinedPythonScriptsMenu
                = new("Pythonスクリプトを実行", new SimpleDelegateCommand<object>((parameter) => {
                    if (MainWindowViewModel.ActiveInstance.SelectedItem == null) {
                        Tools.Error("スクリプトを選択してください");
                        return;
                    }
                    PythonCommands.OpenListPythonScriptWindowExecCommandExecute((scriptItem) => {
                        ClipboardItemViewModel.MenuItemRunPythonScriptCommandExecute(scriptItem, MainWindowViewModel.ActiveInstance.SelectedItem);
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
