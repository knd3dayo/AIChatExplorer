using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public partial class ClipboardFolderViewModel {

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
        public ObservableCollection<MenuItem> CreateBasicItemContextMenuItems(ClipboardItemViewModel itemViewModel) {
            // MenuItemのリストを作成
            ObservableCollection<MenuItem> menuItems = [];
            if (MainWindowViewModel.ActiveInstance == null) {
                return menuItems;
            }
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

            // タイトルを生成
            MenuItem generateTitleMenuItem = new() {
                Header = StringResources.GenerateTitle,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateTitleCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(generateTitleMenuItem);

            // 背景情報生成
            MenuItem generateBackgroundInfoMenuItem = new() {
                Header = StringResources.GenerateBackgroundInfo,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateBackgroundInfoCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(generateBackgroundInfoMenuItem);

            // サマリーを生成
            MenuItem generateSummaryMenuItem = new() {
                Header = StringResources.GenerateSummary,
                // 複数のアイテムの処理を行うため、MainWindowViewModelのコマンドを使用
                Command = MainWindowViewModel.ActiveInstance.GenerateSummaryCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(generateSummaryMenuItem);

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
                Header = StringResources.Pin,
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
        public virtual void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new ClipboardFolderViewModel(MainWindowViewModel, child));
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
            ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems, bool mergeWithHeader) {

            if (selectedItems.Count < 2) {
                LogWrapper.Error(StringResources.SelectTwoItemsToMerge);
                return;
            }
            // マージ先のアイテム。SelectedItems[0]がマージ先
            if (selectedItems[0] is not ClipboardItemViewModel toItemViewModel) {
                LogWrapper.Error(StringResources.MergeTargetNotSelected);
                return;
            }
            List<ClipboardItemViewModel> fromItemsViewModel = [];
            try {
                // toItemにSelectedItems[1]からCount - 1までのアイテムをマージする
                for (int i = 1; i < selectedItems.Count; i++) {
                    if (selectedItems[i] is not ClipboardItemViewModel fromItemModelView) {
                        LogWrapper.Error(StringResources.MergeSourceNotSelected);
                        return;
                    }
                    fromItemsViewModel.Add(fromItemModelView);
                }
                toItemViewModel.MergeItems(fromItemsViewModel, mergeWithHeader);

                // ClipboardItemをLiteDBに保存
                toItemViewModel.SaveClipboardItemCommand.Execute(true);
                // コピー元のアイテムを削除
                foreach (var fromItem in fromItemsViewModel) {
                    fromItem.DeleteItemCommand.Execute();
                }

                // フォルダ内のアイテムを再読み込み
                folderViewModel.LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.Merged);

            } catch (Exception e) {
                string message = $"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}";
                LogWrapper.Error(message);
            }
        }
    }
}
