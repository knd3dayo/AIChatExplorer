using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Common;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using ClipboardApp.ViewModel.Common;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Main;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using WpfAppCommon.Utils;


namespace ClipboardApp.ViewModel.Folders.Clipboard {
    public class ClipboardFolderViewModel(ContentFolder clipboardItemFolder) : ContentFolderViewModel(clipboardItemFolder) {
        public override ClipboardItemViewModel CreateItemViewModel(ContentItem item) {
            return new ClipboardItemViewModel(this, item);
        }

        // RootFolderのViewModelを取得する
        public override ContentFolderViewModel GetRootFolderViewModel() {
            return MainWindowViewModel.Instance.RootFolderViewModelContainer.RootFolderViewModel;
        }


        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ContentFolder childFolder) {
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

        // フォルダ作成コマンドの実装
        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            ClipboardFolder childFolder = (ClipboardFolder)Folder.CreateChild("");
            ClipboardFolderViewModel childFolderViewModel = new(childFolder);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

        public override void LoadFolderExecute(Action beforeAction, Action afterAction) {
            beforeAction();
            Task.Run(() => {
                LoadChildren(DefaultNextLevel);
                LoadItems();
            }).ContinueWith((task) => {
                afterAction();
            });
        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);
        }

        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(Folder.Id);
            ContentItemViewModel ItemViewModel = new ClipboardItemViewModel(this, clipboardItem);


            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, ItemViewModel,
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

        public virtual void PasteClipboardItemCommandExecute(ClipboardController.CutFlagEnum CutFlag,
            IEnumerable<object> items, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                if (item is ClipboardItemViewModel itemViewModel) {
                    ContentItem clipboardItem = itemViewModel.ContentItem;
                    if (CutFlag == ClipboardController.CutFlagEnum.Item) {
                        // Cutフラグが立っている場合はコピー元のアイテムを削除する
                        clipboardItem.MoveToFolder(toFolder.Folder);
                    } else {
                        clipboardItem.CopyToFolder(toFolder.Folder);
                    }
                }
                if (item is ClipboardFolderViewModel folderViewModel) {
                    ClipboardFolder folder = (ClipboardFolder)folderViewModel.Folder;
                    if (CutFlag == ClipboardController.CutFlagEnum.Folder) {
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

        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理


        #endregion


        public override SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            LoadFolderExecute(
                () => {
                    MainWindowViewModel.Instance.UpdateIndeterminate(true);
                },
                () => {
                    MainUITask.Run(() => {
                        MainWindowViewModel.Instance.UpdateIndeterminate(false);
                        UpdateStatusText();
                    });
                });
        });


        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            DeleteDisplayedItemCommandExecute(() => {
                MainWindowViewModel.Instance.UpdateIndeterminate(true);
            }, () => {
                // 全ての削除処理が終了した後、後続処理を実行
                // フォルダ内のアイテムを再読み込む
                MainUITask.Run(() => {
                    LoadFolderCommand.Execute();
                });
                LogWrapper.Info(CommonStringResources.Instance.Deleted);
                MainWindowViewModel.Instance.UpdateIndeterminate(false);
            });
        });


        // ExtractTextCommand
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            // ContentTypes.Files, ContentTypes.Imageのアイテムを取得
            var itemViewModels = Items.Where(x => x.ContentItem.ContentType == ContentTypes.ContentItemTypes.Files || x.ContentItem.ContentType == ContentTypes.ContentItemTypes.Files);

            // MainWindowViewModel.Instance.SelectedItemsにContentTypes.Files, ContentTypes.Imageのアイテムを設定
            MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems = [.. itemViewModels];

            // コマンドを実行
            ClipboardItemViewModelCommands commands = new();
            commands.ExtractTextCommand.Execute();

        });


        //
    }
}
