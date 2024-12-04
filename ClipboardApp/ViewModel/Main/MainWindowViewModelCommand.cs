using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.View.AutoProcessRule;
using ClipboardApp.View.Help;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Main;
using PythonAILib.Model.Prompt;
using QAChat.Resource;
using QAChat.View.AutoGen;
using QAChat.View.PromptTemplate;
using QAChat.View.Tag;
using QAChat.ViewModel.PromptTemplate;
using QAChat.ViewModel.Script;
using WpfAppCommon.Utils;

namespace ClipboardApp {
    public partial class MainWindowViewModel {
        // アプリケーションを終了する。
        // Ctrl + Q が押された時の処理
        // メニューの「終了」をクリックしたときの処理

        public static SimpleDelegateCommand<object> ExitCommand => new((parameter) => {
            ClipboardAppCommandExecute.ExitCommand();
        });


        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleClipboardMonitor => new((parameter) => {
            ClipboardAppCommandExecute.StartStopClipboardMonitorCommand();
        });

        // Windows通知監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleWindowsNotificationMonitor => new((parameter) => {
            ClipboardAppCommandExecute.StartStopWindowsNotificationMonitorCommand();
        });

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
            if (SelectedFolder != null) {
                // Load
                SelectedFolder.LoadFolderCommand.Execute();
            }
        });

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand => new((routedEventArgs) => {
            // ListBoxの場合
            if (routedEventArgs.OriginalSource is ListBox) {
                ListBox listBox = (ListBox)routedEventArgs.OriginalSource;
                ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)listBox.SelectedItem;
                SelectedItem = clipboardItemViewModel;
                // SelectedItemsをMainWindowViewModelにセット
                SelectedItems.Clear();
                foreach (ClipboardItemViewModel item in listBox.SelectedItems) {
                    SelectedItems.Add(item);
                }
            }
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)dataGrid.SelectedItem;
                SelectedItem = clipboardItemViewModel;
                // SelectedItemsをMainWindowViewModelにセット
                SelectedItems.Clear();
                foreach (ClipboardItemViewModel item in dataGrid.SelectedItems) {
                    SelectedItems.Add(item);
                }
            }

        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> ChangePinCommand => new((parameter) => {

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(StringResources.NoItemSelected);
                return;
            }
            foreach (ClipboardItemViewModel clipboardItemViewModel in SelectedItems) {
                clipboardItemViewModel.ChangePinCommand.Execute();
            }
            // フォルダ内のアイテムを再読み込み
            SelectedFolder?.LoadFolderCommand.Execute();
        });


        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public static void OpenListPythonScriptWindowCommandExecute(object parameter) {
            PythonCommands.OpenListPythonScriptWindowCommandExecute(parameter);
        }
        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public static void OpenListPromptTemplateWindowCommandExecute(MainWindowViewModel windowViewModel) {
            // ListPromptTemplateWindowを開く
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Edit, (promptTemplateWindowViewModel, OpenAIExecutionModeEnum) => {
                // PromptTemplate = promptTemplateWindowViewModel.ClipboardPromptItem;
            });
        }
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public void OpenListAutoProcessRuleWindowCommandExecute() {
            // ListAutoProcessRuleWindowを開く
            ListAutoProcessRuleWindow.OpenListAutoProcessRuleWindow(this);

        }
        // メニューの「タグ編集」をクリックしたときの処理
        public static void OpenTagWindowCommandExecute() {
            // TagWindowを開く
            TagWindow.OpenTagWindow(null, () => { });

        }

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListPythonScriptWindowCommand => new((parameter) => {
            OpenListPythonScriptWindowCommandExecute(parameter);
        });

        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListPromptTemplateWindowCommand => new((parameter) => {
            OpenListPromptTemplateWindowCommandExecute(this);
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            OpenListAutoProcessRuleWindowCommandExecute();
        });
        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenTagWindowCommand => new((parameter) => {
            OpenTagWindowCommandExecute();
        });
        // メニューの「AutoGen定義編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoGenItemWindowCommand => new((parameter) => {
            ListAutoGenItemWindow.OpenListAutoGenItemWindow(RootFolderViewModelContainer.RootFolderViewModel);
        });

        // バージョン情報画面を開く処理
        public SimpleDelegateCommand<object> OpenVersionInfoCommand => new((parameter) => {
            VersionWindow.OpenVersionWindow();
        });

        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。
        // チャット履歴フォルダーに新規作成
        public SimpleDelegateCommand<object> OpenOpenAIWindowCommand => new((parameter) => {

            ClipboardItem dummyItem = new(SelectedFolder?.ClipboardItemFolder.Id ?? RootFolderViewModelContainer.RootFolderViewModel.ClipboardItemFolder.Id);
            ClipboardAppCommandExecute.OpenOpenAIChatWindowCommand(dummyItem);

        });

        // OpenScreenshotCheckerWindowExecute メニューの「画像エビデンスチェッカー」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenScreenshotCheckerWindow => new((parameter) => {
            // チャット履歴フォルダーに新規作成
            ClipboardItem dummyItem = new(RootFolderViewModelContainer.ChatRootFolderViewModel.ClipboardItemFolder.Id);
            ClipboardAppCommandExecute.OpenImageChatWindowCommand(dummyItem, () => {
                RootFolderViewModelContainer.ChatRootFolderViewModel.LoadFolderCommand.Execute();
            });
        });

        // OpenVectorSearchWindowCommand メニューの「ベクトル検索」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorSearchWindowCommand => new((parameter) => {
            ClipboardFolderViewModel folderViewModel = SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            ClipboardAppCommandExecute.OpenVectorSearchWindowCommand(folderViewModel.ClipboardItemFolder);
        });

        // OpenRAGManagementWindowCommandメニュー　「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenRAGManagementWindowCommand => new((parameter) => {
            ClipboardAppCommandExecute.OpenRAGManagementWindowCommand();
        });
        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorDBManagementWindowCommand => new((parameter) => {
            ClipboardAppCommandExecute.OpenVectorDBManagementWindowCommand();
        });

        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            ClipboardAppCommandExecute.SettingCommandExecute();
        });


        #region Window全体のInputBinding用のコマンド
        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand<object> SearchCommand => new((parameter) => {
            ClipboardFolderViewModel folderViewModel = SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            ClipboardAppCommandExecute.OpenSearchWindowCommand(folderViewModel.ClipboardItemFolder, () => { folderViewModel.LoadFolderCommand.Execute(); });
        });

        #endregion

        #region フォルダツリーのInputBinding用のコマンド
        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            ClipboardAppCommandExecute.ReloadFolderCommand(this);
        });

        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand<object> CreateItemCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            this.SelectedFolder.CreateItemCommandExecute();
        });

        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand<object> PasteCommand => new((parameter) => {
            ClipboardAppCommandExecute.PasteFromClipboardCommandExecute();
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutFolderCommand => new((parameter) => {
            ClipboardAppCommandExecute.CutFolderCommandExecute(this);
        });

        #endregion

        #region クリップボードアイテムのInputBinding用のコマンド
        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            ClipboardFolderViewModel.DeleteDisplayedItemCommandExecute(SelectedFolder);
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(StringResources.ConfirmDeleteSelectedItems, StringResources.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                // 選択中のアイテムを削除
                foreach (var item in SelectedItems) {
                    item.DeleteItemCommand.Execute();
                }
                // フォルダ内のアイテムを再読み込む
                SelectedFolder?.LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.Deleted);
            }
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutItemCommand => new((parameter) => {
            ClipboardAppCommandExecute.CutItemCommandExecute(this);
        });
        // Ctrl + C が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CopyItemCommand => new((parameter) => {
            ClipboardAppCommandExecute.CopyToClipboardCommandExecute(this);
        });
        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemCommand => new((parameter) => {
            ClipboardAppCommandExecute.MergeItemCommandExecute(this);
        });

        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemWithHeaderCommand => new((parameter) => {
            ClipboardAppCommandExecute.MergeItemWithHeaderCommandExecute(this);
        });

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            this.SelectedFolder?.OpenItemCommand.Execute(this.SelectedItem);

        });
        #endregion

        #region クリップボードアイテムのコンテキストメニューのInputBinding用のコマンド
        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((parameter) => {
            this.SelectedItem?.OpenContentAsFileCommand.Execute();
        });

        // 選択したアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenFileCommand => new((parameter) => {
            this.SelectedItem?.OpenFileCommand.Execute();
        });

        // タイトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateTitleCommand => new((parameter) => {
            ClipboardAppCommandExecute.GenerateTitleCommand(SelectedItems.Select(x => x.ClipboardItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    SelectedFolder?.LoadFolderCommand.Execute();
                });
            });
        });

        // 背景情報を生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateBackgroundInfoCommand => new((parameter) => {
            ClipboardAppCommandExecute.GenerateBackgroundInfoCommand(SelectedItems.Select(x => x.ClipboardItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    SelectedFolder?.LoadFolderCommand.Execute();
                });
            });
        });

        // サマリーを生成する処理　複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateSummaryCommand => new((parameter) => {
            ClipboardAppCommandExecute.GenerateSummaryCommand(SelectedItems.Select(x => x.ClipboardItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    SelectedFolder?.LoadFolderCommand.Execute();
                });
            });
        });

        // 課題リストを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateTasksCommand => new((parameter) => {
            ClipboardAppCommandExecute.GenerateTasksCommand(SelectedItems.Select(x => x.ClipboardItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    SelectedFolder?.LoadFolderCommand.Execute();
                });
            });
        });
        // 文書の信頼度を判定する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> CheckDocumentReliabilityCommand => new((parameter) => {
            ClipboardAppCommandExecute.CheckDocumentReliabilityCommand(SelectedItems.Select(x => x.ClipboardItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    SelectedFolder?.LoadFolderCommand.Execute();
                });
            });
        });


        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateVectorCommand => new((parameter) => {
            ClipboardAppCommandExecute.GenerateVectorCommand(SelectedItems.Select(x => x.ClipboardItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    SelectedFolder?.LoadFolderCommand.Execute();
                });
            });
        });

        // ベクトル検索を実行する処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> VectorSearchCommand => new((parameter) => {
            this.SelectedItem?.VectorSearchCommand.Execute();
        });


        // プロンプトテンプレートを実行
        public SimpleDelegateCommand<Tuple<ClipboardItemViewModel, PromptItem>> ExecutePromptTemplateCommand => new((tuple) => {
            ClipboardItemViewModel itemViewModel = tuple.Item1;
            PromptItem promptItem = tuple.Item2;
            // チャットを実行
            Task.Run(() => {
                itemViewModel.ClipboardItem.CreateChatResult(promptItem);
                //保存
                itemViewModel.ClipboardItem.Save();
                MainUITask.Run(() => {
                    // フォルダ内のアイテムを再読み込み
                    SelectedFolder?.LoadFolderCommand.Execute();
                });
            });
        });

        #endregion

    }
}
