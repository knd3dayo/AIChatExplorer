using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using PythonAILib.Resource;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public partial class ClipboardFolderViewModel {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public virtual ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            return new ClipboardFolderViewModel(MainWindowViewModel, childFolder);
        }
        // -- virtual
        public virtual ObservableCollection<MenuItem> MenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new() {
                    Header = StringResources.Create,
                    Command = CreateFolderCommand,
                    CommandParameter = this
                };
                menuItems.Add(createMenuItem);

                // 編集
                MenuItem editMenuItem = new() {
                    Header = StringResources.Edit,
                    Command = EditFolderCommand,
                    IsEnabled = IsEditVisible,
                    CommandParameter = this
                };
                menuItems.Add(editMenuItem);

                // 削除
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = DeleteFolderCommand;
                deleteMenuItem.IsEnabled = IsDeleteVisible;
                deleteMenuItem.CommandParameter = this;
                menuItems.Add(deleteMenuItem);

                // エクスポート/インポート
                MenuItem exportImportMenuItem = new() {
                    Header = StringResources.ExportImport,
                    Command = ExportImportFolderCommand,
                    CommandParameter = this
                };
                menuItems.Add(exportImportMenuItem);


                // アイテムのバックアップ/リストア
                MenuItem backupRestoreMenuItem = new() {
                    Header = StringResources.BackupRestore
                };

                // バックアップ
                MenuItem backupMenuItem = new() {
                    Header = StringResources.BackupItem,
                    Command = BackupItemsFromFolderCommand,
                    CommandParameter = this
                };
                backupRestoreMenuItem.Items.Add(backupMenuItem);


                // リストア
                MenuItem restoreMenuItem = new() {
                    Header = StringResources.RestoreItem,
                    Command = RestoreItemsToFolderCommand,
                    CommandParameter = this
                };
                backupRestoreMenuItem.Items.Add(restoreMenuItem);

                menuItems.Add(backupRestoreMenuItem);

                return menuItems;

            }
        }

        public MenuItem CreatePromptMenuItems(ClipboardItemViewModel itemViewModel) {
            // プロンプトメニュー
            MenuItem promptMenuItem = new() {
                Header = StringResources.PromptMenu,
            };
            // タイトルを生成
            MenuItem generateTitleMenuItem = new() {
                Header = StringResources.GenerateTitle,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateTitleCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateTitleMenuItem);

            // 背景情報生成
            MenuItem generateBackgroundInfoMenuItem = new() {
                Header = StringResources.GenerateBackgroundInfo,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateBackgroundInfoCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateBackgroundInfoMenuItem);

            // サマリーを生成
            MenuItem generateSummaryMenuItem = new() {
                Header = StringResources.GenerateSummary,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateSummaryCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateSummaryMenuItem);

            // 課題リストを生成
            MenuItem generateTasksMenuItem = new() {
                Header = StringResources.GenerateTasks,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateTasksCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(generateTasksMenuItem);

            // その他のプロンプト(プロンプトテンプレート一覧画面を開く)
            MenuItem otherPromptMenuItem = new() {
                Header = StringResources.OtherPrompts,
                Command = MainWindowViewModel.ActiveInstance.SelectPromptTemplateCommand,
                CommandParameter = itemViewModel
            };
            promptMenuItem.Items.Add(otherPromptMenuItem);

            return promptMenuItem;
        }

        public ObservableCollection<MenuItem> CreateBasicItemContextMenuItems(ClipboardItemViewModel itemViewModel) {
            // MenuItemのリストを作成
            ObservableCollection<MenuItem> menuItems = [];
            // 開く
            MenuItem createMenuItem = new() {
                Header = StringResources.Open,
                Command = OpenItemCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Ctrl+O"
            };
            menuItems.Add(createMenuItem);

            // テキストをファイルとして開く
            MenuItem openContentAsFileMenuItem = new() {
                Header = StringResources.OpenTextAsFile,
                Command = itemViewModel.OpenContentAsFileCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Ctrl+Shit+O"
            };
            menuItems.Add(openContentAsFileMenuItem);
            // プロンプトメニュー
            MenuItem promptMenuItem = CreatePromptMenuItems(itemViewModel);
            menuItems.Add(promptMenuItem);

            // ベクトル生成
            MenuItem generateVectorMenuItem = new() {
                Header = StringResources.GenerateVector,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateVectorCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(generateVectorMenuItem);

            // ベクトル検索
            MenuItem vectorSearchMenuItem = new() {
                Header = StringResources.VectorSearch,
                // 将来、複数のアイテムの処理を行う可能性があるため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.VectorSearchCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(vectorSearchMenuItem);

            // ピン留め
            MenuItem pinnedStateChangeMenuItem = new() {
                Header = PythonAILibStringResources.Instance.Pin,
                Command = itemViewModel.ChangePinCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(pinnedStateChangeMenuItem);

            // コピー
            MenuItem copyMenuItem = new() {
                Header = StringResources.Copy,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.CopyItemCommand,
                CommandParameter = this,
                InputGestureText = "Ctrl+C"
            };
            menuItems.Add(copyMenuItem);

            // 削除
            MenuItem deleteMnuItem = new() {
                Header = StringResources.Delete,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.DeleteSelectedItemCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Delete"
            };
            menuItems.Add(deleteMnuItem);

            return menuItems;
        }
        // Itemのコンテキストメニュー
        public virtual ObservableCollection<MenuItem> CreateItemContextMenuItems(ClipboardItemViewModel itemViewModel) {
            return CreateBasicItemContextMenuItems(itemViewModel);
        }


        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        public void LoadChildren(int nestLevel=5) {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                ClipboardFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                if (nestLevel > 0) {
                    childViewModel.LoadChildren(nestLevel - 1);
                }
                Children.Add(childViewModel);
            }

        }
        // LoadItems
        public virtual void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(this, item));
            }
        }


        // フォルダ作成コマンドの実装
        public virtual void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild("");
            childFolder.FolderType = ClipboardFolder.FolderTypeEnum.Normal;
            ClipboardFolderViewModel childFolderViewModel = new(MainWindowViewModel, childFolder);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {

            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);

        }

        public virtual void CreateItemCommandExecute() {
            EditItemWindow.OpenEditItemWindow(this, null, () => {
                // フォルダ内のアイテムを再読み込み
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.Added);
            });
        }
        public virtual void OpenItemCommandExecute(ClipboardItemViewModel item) {
            EditItemWindow.OpenEditItemWindow(this, item, () => {
                // フォルダ内のアイテムを再読み込み
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.Edited);
            });
        }

        /// <summary>
        /// Ctrl + V が押された時の処理
        /// コピー中のアイテムを選択中のフォルダにコピー/移動する
        /// 貼り付け後にフォルダ内のアイテムを再読み込む
        /// 
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="item"></param>
        /// <param name="fromFolder"></param>
        /// <param name="toFolder"></param>
        /// <returns></returns>

        public virtual void PasteClipboardItemCommandExecute(bool CutFlag,
            IEnumerable<ClipboardItemViewModel> items, ClipboardFolderViewModel fromFolder, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                ClipboardItemViewModel newItem = item.Copy();
                toFolder.AddItemCommand.Execute(newItem);
                // Cutフラグが立っている場合はコピー元のアイテムを削除する
                if (CutFlag) {
                    fromFolder.DeleteItemCommand.Execute(item);
                }
            }
            // フォルダ内のアイテムを再読み込み
            toFolder.LoadFolderCommand.Execute();
            LogWrapper.Info(StringResources.Pasted);
        }

        public virtual void MergeItemCommandExecute(
            ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems) {

            if (selectedItems.Count < 2) {
                LogWrapper.Error(StringResources.SelectTwoItemsToMerge);
                return;
            }
            selectedItems[0].MergeItems([.. selectedItems]);

            // フォルダ内のアイテムを再読み込み
            folderViewModel.LoadFolderCommand.Execute();
            LogWrapper.Info(StringResources.Merged);

        }
    }
}
