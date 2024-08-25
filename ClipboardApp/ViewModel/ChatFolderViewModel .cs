using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using PythonAILib.Model;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Model;
using WpfAppCommon.Model.ClipboardApp;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
    public class ChatFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(mainWindowViewModel, clipboardItemFolder) {
        public override ObservableCollection<MenuItem> MenuItems {
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

                // アイテムのバックアップ/リストア
                MenuItem backupRestoreMenuItem = new();
                backupRestoreMenuItem.Header = StringResources.BackupRestore;

                // バックアップ
                MenuItem backupMenuItem = new() {
                    Header = StringResources.BackupItem,
                    Command = BackupItemsFromFolderCommand,
                    CommandParameter = this
                };
                backupRestoreMenuItem.Items.Add(backupMenuItem);

                menuItems.Add(backupRestoreMenuItem);

                return menuItems;

            }
        }

        // LoadChildren
        public override void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new ChatFolderViewModel(MainWindowViewModel, child));
            }

        }
        // LoadItems
        public override void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(this, item));
            }
        }

        // アイテム作成コマンドの実装. 画像チェックの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(ClipboardItemFolder.Id);
            ClipboardItemViewModel clipboardItemViewModel = new(this, clipboardItem);
            OpenItemCommandExecute(clipboardItemViewModel);
        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel itemViewModel) {
            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatStartupProps props = new(ClipboardItemFolder, itemViewModel.ClipboardItem) {
                // ベクトルDBアイテムを開くアクション
                OpenVectorDBItemAction = (vectorDBItem) => {
                    VectorDBItemViewModel vectorDBItemViewModel = new(vectorDBItem);
                    EditVectorDBWindow.OpenEditVectorDBWindow(vectorDBItemViewModel, (model) => { });
                },
                // ベクトルDBアイテムを選択するアクション
                SelectVectorDBItemsAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (selectedItem) => {
                        vectorDBItems.Add(selectedItem);
                    });
                },
                // フォルダ選択アクション
                SelectFolderAction = (vectorDBItems) => {
                    if (MainWindowViewModel.ActiveInstance == null) {
                        LogWrapper.Error("MainWindowViewModelがNullです");
                        return;
                    }
                    FolderSelectWindow.OpenFolderSelectWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, (folderViewModel) => {
                        vectorDBItems.Add(folderViewModel.ClipboardItemFolder.GetVectorDBItem());
                    });

                },
                // 選択中のクリップボードアイテムを取得するアクション
                GetSelectedClipboardItemImageFunction = () => {
                    List<ImageItemBase> images = [];
                    var selectedItems = MainWindowViewModel.ActiveInstance?.SelectedItems;
                    if (selectedItems == null) {
                        return images;
                    }
                    foreach (ClipboardItemViewModel selectedItem in selectedItems) {
                        selectedItem.ClipboardItem.ClipboardItemImages.ForEach((image) => {
                            images.Add(image);
                        });
                    }
                    return images;
                }

            };

            QAChat.QAChatMainWindow.OpenOpenAIChatWindow(props);
        }

        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックの場合は、画像チェックを作成
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild("");
            childFolder.FolderType = ClipboardFolder.FolderTypeEnum.ImageCheck;
            ImageCheckFolderViewModel childFolderViewModel = new(MainWindowViewModel, childFolder);

            // TODO チャット履歴作成画面を開くようにする。フォルダ名とRAGソースのリストを選択可能にする。
            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }
        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public override void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);
        }

    }
}

