using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SearchView;
using QAChat.View.VectorDBWindow;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using static QAChat.MainWindowViewModel;

namespace ClipboardApp.View.ClipboardItemFolderView {
    public class ChatFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(mainWindowViewModel, clipboardItemFolder) {
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new();
                createMenuItem.Header = StringResources.Create;
                createMenuItem.Command = CreateFolderCommand;
                createMenuItem.CommandParameter = this;
                menuItems.Add(createMenuItem);

                // 編集
                MenuItem editMenuItem = new();
                editMenuItem.Header = StringResources.Edit;
                editMenuItem.Command = EditFolderCommand;
                editMenuItem.IsEnabled = IsEditVisible;
                editMenuItem.CommandParameter = this;
                menuItems.Add(editMenuItem);

                // 削除
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = DeleteFolderCommand;
                deleteMenuItem.IsEnabled = IsDeleteVisible;
                deleteMenuItem.CommandParameter = this;
                menuItems.Add(deleteMenuItem);

                // インポート    
                MenuItem importMenuItem = new();
                importMenuItem.Header = StringResources.Import;
                importMenuItem.Command = ImportItemsToFolderCommand;
                importMenuItem.CommandParameter = this;
                menuItems.Add(importMenuItem);

                // エクスポート
                MenuItem exportMenuItem = new();
                exportMenuItem.Header = StringResources.Export;
                exportMenuItem.Command = ExportItemsFromFolderCommand;
                exportMenuItem.CommandParameter = this;
                menuItems.Add(exportMenuItem);

                return menuItems;

            }
        }
        // Itemのコンテキストメニュー
        public override ObservableCollection<MenuItem> ItemContextMenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                if (MainWindowViewModel.ActiveInstance == null) {
                    return menuItems;
                }
                // 開く
                MenuItem createMenuItem = new();
                createMenuItem.Header = "開く";
                createMenuItem.Command = MainWindowViewModel.ActiveInstance.OpenSelectedItemCommand;
                createMenuItem.CommandParameter = this;
                createMenuItem.InputGestureText = "Ctrl+O";
                menuItems.Add(createMenuItem);

                // ピン留め
                MenuItem pinnedStateChangeMenuItem = new();
                pinnedStateChangeMenuItem.Header = "ピン留め";
                pinnedStateChangeMenuItem.Command = MainWindowViewModel.ActiveInstance.ChangePinCommand;
                pinnedStateChangeMenuItem.CommandParameter = this;
                menuItems.Add(pinnedStateChangeMenuItem);

                // コピー
                MenuItem copyMenuItem = new();
                copyMenuItem.Header = "コピー";
                copyMenuItem.Command = MainWindowViewModel.ActiveInstance.CopyItemCommand;
                copyMenuItem.CommandParameter = this;
                copyMenuItem.InputGestureText = "Ctrl+C";
                menuItems.Add(copyMenuItem);

                // 削除
                MenuItem deleteMnuItem = new();
                deleteMnuItem.Header = "削除";
                deleteMnuItem.Command = MainWindowViewModel.ActiveInstance.DeleteSelectedItemCommand;
                deleteMnuItem.CommandParameter = this;
                deleteMnuItem.InputGestureText = "Delete";
                menuItems.Add(deleteMnuItem);

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
            ClipboardItem clipboardItem = new(this.ClipboardItemFolder.Id);
            ClipboardItemViewModel clipboardItemViewModel = new(this, clipboardItem);
            OpenItemCommandExecute(clipboardItemViewModel);
        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel itemViewModel) {
            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatStartupProps props = new(this.ClipboardItemFolder, itemViewModel.ClipboardItem, false) {
                // ベクトルDBアイテムを開くアクション
                OpenVectorDBItemAction = (vectorDBItem) => {
                    VectorDBItemViewModel vectorDBItemViewModel = new(vectorDBItem);
                    EditVectorDBWindow.OpenEditVectorDBWindow(vectorDBItemViewModel, (model) => { });
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
                    List<ClipboardItemImage> images = [];
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

            QAChat.MainWindow.OpenOpenAIChatWindow(props);
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

