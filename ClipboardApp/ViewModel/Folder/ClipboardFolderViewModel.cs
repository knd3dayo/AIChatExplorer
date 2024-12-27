using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItem;
using ClipboardApp.View.Folder;
using ClipboardApp.ViewModel.Common;
using ClipboardApp.ViewModel.Content;
using PythonAILib.Model.Content;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;


namespace ClipboardApp.ViewModel {
    public class ClipboardFolderViewModel(ClipboardFolder clipboardItemFolder) : Folder.ClipboardFolderBase<ClipboardItemViewModel>(clipboardItemFolder) {
        public override ClipboardItemViewModel CreateItemViewModel(ContentItem item) {
            return new ClipboardItemViewModel(this, item);
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            var childFolderViewModel = new ClipboardFolderViewModel(childFolder) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ClipboardFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        public bool IsNameEditable {
            get {
                return Folder.IsRootFolder == false;
            }

        }
        // フォルダ作成コマンドの実装
        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            ClipboardFolder childFolder = (ClipboardFolder)Folder.CreateChild("");
            childFolder.FolderType = FolderTypeEnum.Normal;
            ClipboardFolderViewModel childFolderViewModel = new(childFolder);

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

        public override void CreateItemCommandExecute() {
            /**
            EditItemWindow.OpenEditItemWindow(this, null, () => {
                // フォルダ内のアイテムを再読み込み
                LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.Added);
            });
            **/
            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, null,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    LoadFolderCommand.Execute();
                    LogWrapper.Info(StringResources.Edited);
                });

            ClipboardAppTabContainer container = new("New Item", editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.AddTabItem(container);


        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel item) {
            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, item,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    LoadFolderCommand.Execute();
                    LogWrapper.Info(StringResources.Edited);
                });

            ClipboardAppTabContainer container = new(item.ClipboardItem.Description, editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.AddTabItem(container);


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

        public override void PasteClipboardItemCommandExecute(MainWindowViewModel.CutFlagEnum CutFlag,
            IEnumerable<object> items, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                if (item is ClipboardItemViewModel itemViewModel) {
                    ContentItem clipboardItem = itemViewModel.ClipboardItem;
                    if (CutFlag == MainWindowViewModel.CutFlagEnum.Item) {
                        // Cutフラグが立っている場合はコピー元のアイテムを削除する
                        clipboardItem.MoveToFolder(toFolder.Folder);
                    } else {
                        clipboardItem.CopyToFolder(toFolder.Folder);
                    }
                }
                if (item is ClipboardFolderViewModel folderViewModel) {
                    ClipboardFolder folder = (ClipboardFolder)folderViewModel.Folder;
                    if (CutFlag == MainWindowViewModel.CutFlagEnum.Folder) {
                        // Cutフラグが立っている場合はコピー元のフォルダを削除する
                        folder.MoveTo(toFolder.Folder);
                        // 元のフォルダの親フォルダを再読み込み
                        folderViewModel.ParentFolderViewModel?.LoadFolderCommand.Execute();
                    }
                }

            }
            toFolder.LoadFolderCommand.Execute();

            LogWrapper.Info(StringResources.Pasted);
        }

        public override void MergeItemCommandExecute(
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
