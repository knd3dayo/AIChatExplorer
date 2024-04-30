using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.AutoProcessRuleView;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using ClipboardApp.View.SettingWindow;
using ClipboardApp.View.StatusMessageView;
using ClipboardApp.View.TagView;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.RAGWindow;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp {
    public class MainWindowCommand {

        public static void ExitCommand() {
            // 終了確認ダイアログを表示。Yesならアプリケーションを終了
            MessageBoxResult result = MessageBox.Show("終了しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                Application.Current.Shutdown();
            }
        }
        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public static void ToggleClipboardMonitorCommand(MainWindowViewModel windowViewModel) {
            windowViewModel.IsClipboardMonitor = !windowViewModel.IsClipboardMonitor;
            windowViewModel.NotifyPropertyChanged(nameof(windowViewModel.ClipboardMonitorButtonText));

        }

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public static void FolderSelectionChangedCommand(MainWindowViewModel windowViewModel, object parameter) {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;
            windowViewModel.SelectedFolder = clipboardItemFolderViewModel;
        }

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public static void ClipboardItemSelectionChangedCommand(MainWindowViewModel windowViewModel, object parameter) {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            ListBox listBox = (ListBox)routedEventArgs.OriginalSource;
            ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)listBox.SelectedItem;
            windowViewModel.SelectedItems.Clear();
            foreach (ClipboardItemViewModel item in listBox.SelectedItems) {
                windowViewModel.SelectedItems.Add(item);
            }
            windowViewModel.SelectedItem = clipboardItemViewModel;
        }
        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public static void CreateItemCommand(MainWindowViewModel windowViewModel, object parameter) {
            // 選択中のフォルダがない場合は処理をしない
            if (windowViewModel.SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemCommands.CreateItemCommandExecute(windowViewModel.SelectedFolder);
        }


        // OpenOpenAIWindowCommand メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public static void OpenOpenAIWindowCommand() {
            ClipboardItemCommands.OpenOpenAIChatWindowExecute(null);
        }
        // OpenRAGManagementWindowCommand メニューの「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public static void OpenRAGManagementWindowCommand() {
            // RARManagementWindowを開く
            RagManagementWindow ragManagementWindow = new ();
            RAGManagementWindowViewModel ragManagementWindowViewModel = (RAGManagementWindowViewModel)ragManagementWindow.DataContext;
            ragManagementWindowViewModel.Initialize();
            ragManagementWindow.ShowDialog();

        }

        // Ctrl + F が押された時の処理
        public static void SearchCommand(MainWindowViewModel windowViewModel) {
            // 選択中のフォルダがない場合でも処理をする
            ClipboardFolderCommands.SearchCommandExecute(windowViewModel.SelectedFolder);
        }

        public static void ReloadCommand(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            if (SelectedFolder == null) {
                return;
            }
            // -- 処理完了までProgressIndicatorを表示
            try {
                windowViewModel.IsIndeterminate = true;
                ClipboardFolderCommands.ReloadCommandExecute(SelectedFolder);
            } finally {
                windowViewModel.IsIndeterminate = false;
            }
        }


        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public static void DeleteDisplayedItemCommand(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            if (SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardFolderCommands.DeleteDisplayedItemCommandExecute(SelectedFolder);
        }

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public static void DeleteSelectedItemCommand(MainWindowViewModel windowViewModel) {
            // 選択中のアイテムがない場合は処理をしない
            if (windowViewModel.SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            if (windowViewModel.SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }

            ClipboardItemCommands.DeleteSelectedItemCommandExecute(windowViewModel.SelectedFolder, windowViewModel.SelectedItems);
        }

        // メニューの「設定」をクリックしたときの処理
        public static void SettingCommand() {
            // 簡易版
            // SimpleSettingWindow settingWindow = new SimpleSettingWindow();
            SettingWindow settingWindow = new();
            settingWindow.ShowDialog();
        }

        // ピン留めの切り替え処理 複数アイテム処理可能
        public static void ChangePinCommand(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.ChangePinCommandExecute(SelectedFolder, SelectedItems);
        }

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public static void OpenSelectedItemCommand(MainWindowViewModel windowViewModel) {
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.OpenItemCommandExecute(SelectedFolder, SelectedItem);
        }


        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public static void OpenSelectedItemAsFileCommand(MainWindowViewModel windowViewModel) {
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItemCommands.OpenSelectedItemAsFileCommandExecute(SelectedItem);
        }

        // 選択したアイテムを新規として開く処理 複数アイテム処理不可
        public static void OpenSelectedItemAsNewFileCommand(MainWindowViewModel windowViewModel) {
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItemCommands.OpenSelectedItemAsNewFileCommandExecute(SelectedItem);
        }

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public static void CutItemCommand(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            List<ClipboardItemViewModel> CopiedItems = windowViewModel.CopiedItems;
            ClipboardFolderViewModel? CopiedItemFolder = windowViewModel.CopiedItemFolder;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            // Cut Flagを立てる
            windowViewModel.CutFlag = true;
            // CopiedItemsに選択中のアイテムをセット
            CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                CopiedItems.Add(item);
            }
            CopiedItemFolder = SelectedFolder;
            Tools.Info("切り取りしました");

        }
        // Ctrl + C が押された時の処理
        public static void CopyToClipboardCommand(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            List<ClipboardItemViewModel> CopiedItems = windowViewModel.CopiedItems;
            ClipboardFolderViewModel? CopiedItemFolder = windowViewModel.CopiedItemFolder;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            // Cutフラグをもとに戻す
            windowViewModel.CutFlag = false;
            // CopiedItemsに選択中のアイテムをセット
            CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                CopiedItems.Add(item);
            }
            CopiedItemFolder = SelectedFolder;
            try {
                ClipboardAppFactory.Instance.GetClipboardController().SetDataObject(SelectedItem.ClipboardItem);
                Tools.Info("コピーしました");

            } catch (Exception e) {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                Tools.Error(message);
            }

        }
        // Ctrl + V が押された時の処理
        public static void PasteFromClipboardCommand(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            List<ClipboardItemViewModel> CopiedItems = windowViewModel.CopiedItems;
            ClipboardFolderViewModel? CopiedItemFolder = windowViewModel.CopiedItemFolder;

            // コピー元のアイテムがない場合は処理をしない
            if (CopiedItems.Count == 0) {
                Tools.Info("コピー元のアイテムがない");
                return;
            }
            // コピー元のフォルダがない場合は処理をしない
            if (CopiedItemFolder == null) {
                Tools.Error("コピー元のフォルダがない");
                return;
            }
            // 貼り付け先のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("貼り付け先のフォルダがない");
                return;
            }
            ClipboardItemCommands.PasteClipboardItemCommandExecute(
                windowViewModel.CutFlag,
                CopiedItems,
                CopiedItemFolder,
                SelectedFolder
                );
            // Cutフラグをもとに戻す
            windowViewModel.CutFlag = false;
            // 貼り付け後にコピー選択中のアイテムをクリア
            CopiedItems.Clear();
            CopiedItemFolder = null;

        }
        // Ctrl + M が押された時の処理
        public static void MergeItemCommand(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                false
                );
        }
        // Ctrl + Shift + M が押された時の処理
        public static void MergeItemWithHeaderCommand(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                true
                );
        }

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public static void OpenListPythonScriptWindowCommand(object parameter) {
            PythonCommands.OpenListPythonScriptWindowCommandExecute(parameter);
        }
        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public static void OpenListPromptTemplateWindowCommand(MainWindowViewModel windowViewModel) {
            ListPromptTemplateWindow listPromptTemplateWindow = new ();
            ListPromptTemplateWindowViewModel listPromptTemplateWindowViewModel = (ListPromptTemplateWindowViewModel)listPromptTemplateWindow.DataContext;
            listPromptTemplateWindowViewModel.Initialize(
                ListPromptTemplateWindowViewModel.ActionModeEum.Edit,
                (promptTemplateWindowViewModel, OpenAIExecutionModeEnum) => {
                    // PromptTemplate = promptTemplateWindowViewModel.PromptItem;
                });
            listPromptTemplateWindow.ShowDialog();
        }
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public static void OpenListAutoProcessRuleWindowCommand() {
            ListAutoProcessRuleWindow listAutoProcessRuleWindow = new ();
            ListAutoProcessRuleWindowViewModel ListAutoProcessRuleWindowViewModel = (ListAutoProcessRuleWindowViewModel)listAutoProcessRuleWindow.DataContext;
                      
            ListAutoProcessRuleWindowViewModel.Initialize(MainWindowViewModel.RootFolderViewModel);

            listAutoProcessRuleWindow.ShowDialog();

        }
        // メニューの「タグ編集」をクリックしたときの処理
        public static void OpenTagWindowCommand() {
            TagWindow tagWindow = new ();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            tagWindowViewModel.Initialize(null, () => { });
            tagWindow.ShowDialog();

        }
        // ステータスバーをクリックしたときの処理
        public static void OpenStatusMessageWindowCommand() {
            StatusMessageWindow statusMessageWindow = new ();
            StatusMessageWindowViewModel statusMessageWindowViewModel = (StatusMessageWindowViewModel)statusMessageWindow.DataContext;
            statusMessageWindowViewModel.Initialize();
            statusMessageWindow.ShowDialog();

        }

        // コンテキストメニューの「ファイルのパスを分割」の実行用コマンド
        public static void SplitFilePathCommand(MainWindowViewModel windowViewModel) {
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            // 選択中のアイテムを取得
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItem clipboardItem = SelectedItem.ClipboardItem;
            // 処理が終わるまでProgressIndicatorを表示
            try {
                windowViewModel.IsIndeterminate = true;
                // ファイルパスを分割
                ClipboardItem.SplitFilePathCommandExecute(clipboardItem);
                // 保存
                clipboardItem.Save();
                // 再描写
                windowViewModel.ReloadClipboardItems();
            } finally {
                windowViewModel.IsIndeterminate = false;
            }
        }


    }
}
