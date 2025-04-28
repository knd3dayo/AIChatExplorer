using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Main;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.File;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Content;


namespace AIChatExplorer.ViewModel.Folders.Clipboard {
    public class ClipboardFolderViewModel(ContentFolderWrapper clipboardItemFolder, ContentItemViewModelCommands commands) : ContentFolderViewModel(clipboardItemFolder, commands) {
        public override ClipboardItemViewModel CreateItemViewModel(ContentItemWrapper item) {
            return new ClipboardItemViewModel(this, item);
        }

        // RootFolderのViewModelを取得する
        public override ContentFolderViewModel GetRootFolderViewModel() {
            return MainWindowViewModel.Instance.RootFolderViewModelContainer.RootFolderViewModel;
        }


        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var childFolderViewModel = new ClipboardFolderViewModel(childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadItems
        public override void LoadItems() {
            LoadItems<ClipboardItem>();
        }

        // LoadChildren
        public override void LoadChildren(int nestLevel) {
            LoadChildren<ClipboardFolderViewModel, ClipboardFolder>(nestLevel);
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
            ClipboardFolderViewModel childFolderViewModel = new(childFolder, Commands);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

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
            ClipboardItem clipboardItem = new(Folder.Entity);
            ContentItemViewModel ItemViewModel = CreateItemViewModel(clipboardItem);


            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, ItemViewModel,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Edited);
                });

            AppTabContainer container = new("New Item", editItemControl);

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
                    ContentItemWrapper clipboardItem = itemViewModel.ContentItem;
                    if (CutFlag == ClipboardController.CutFlagEnum.Item) {
                        // Cutフラグが立っている場合はコピー元のアイテムを削除する
                        clipboardItem.MoveTo(toFolder.Folder);
                    } else {
                        clipboardItem.CopyToFolder(toFolder.Folder);
                    }
                }
                if (item is ClipboardFolderViewModel folderViewModel) {
                    ContentFolderWrapper folder = folderViewModel.Folder;
                    if (CutFlag == ClipboardController.CutFlagEnum.Folder) {
                        // Cutフラグが立っている場合はコピー元のフォルダを削除する
                        folder.MoveTo(toFolder.Folder);
                        // 元のフォルダの親フォルダを再読み込み
                        folderViewModel.ParentFolderViewModel?.LoadFolderCommand.Execute();
                    }
                }

            }
            toFolder.LoadFolderCommand.Execute();

            LogWrapper.Info(CommonStringResources.Instance.Pasted);
        }

        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理


        // ExtractTextCommand
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            // ContentTypes.Files, ContentTypes.Imageのアイテムを取得
            var itemViewModels = Items.Where(x => x.ContentItem.ContentType == ContentTypes.ContentItemTypes.Files || x.ContentItem.ContentType == ContentTypes.ContentItemTypes.Files);
            Commands.ExtractTextCommand.Execute(MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel?.SelectedItems);

        });


        #endregion

        //
    }
}
