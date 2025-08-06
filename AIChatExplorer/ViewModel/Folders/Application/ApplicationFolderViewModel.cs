using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Main;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;


namespace AIChatExplorer.ViewModel.Folders.Application {
    public class ApplicationFolderViewModel(ContentFolderWrapper applicationItemFolder, CommonViewModelCommandExecutes Commands) : ContentFolderViewModel(applicationItemFolder, Commands) {
        public override ApplicationItemViewModel CreateItemViewModel(ContentItemWrapper item) {
            return new ApplicationItemViewModel(this, item);
        }

        // RootFolderのViewModelを取得する
        public override ContentFolderViewModel? GetRootFolderViewModel() {

            return MainWindowViewModel.Instance.RootFolderViewModelContainer.GetApplicationRootFolderViewModel();
        }

        public override ObservableCollection<ContentItemViewModel> GetSelectedItems() {
            // MainWindowViewModelのSelectedItemsを取得
            return MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel?.SelectedItems ?? new ObservableCollection<ContentItemViewModel>();
        }

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override ApplicationFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var childFolderViewModel = new ApplicationFolderViewModel(childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadLLMConfigListAsync
        public override async Task LoadItemsAsync() {
            await LoadItemsAsync<ApplicationItem>();
        }

        // LoadChildren
        public override async Task LoadChildren(int nestLevel) {
            await LoadChildren<ApplicationFolderViewModel, ApplicationFolder>(nestLevel);
        }

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ApplicationFolderMenu applicationItemMenu = new(this);
                return applicationItemMenu.MenuItems;
            }
        }


        public override void CreateItemCommandExecute() {
            ApplicationItem applicationItem = new(Folder.Entity);
            ContentItemViewModel ItemViewModel = CreateItemViewModel(applicationItem);


            // MainWindowViewModelのTabItemを追加する
            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(this, ItemViewModel,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    FolderCommands.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Edited);
                });

            MainTabContent container = new("New Item", editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.MainTabManager.RemoveTabItem(container);
            });
            MainWindowViewModel.Instance.MainTabManager.AddTabItem(container);
        }

        public virtual async Task PasteApplicationItemCommandExecute(ClipboardController.CutFlagEnum CutFlag,
            IEnumerable<object> items, ApplicationFolderViewModel toFolder) {
            foreach (var item in items) {
                if (item is ApplicationItemViewModel itemViewModel) {
                    ContentItemWrapper applicationItem = itemViewModel.ContentItem;
                    if (CutFlag == ClipboardController.CutFlagEnum.Item) {
                        // Cutフラグが立っている場合はコピー元のアイテムを削除する
                        await applicationItem.MoveToAsync(toFolder.Folder);
                    } else {
                        await applicationItem.CopyToFolderAsync(toFolder.Folder);
                    }
                }
                if (item is ApplicationFolderViewModel folderViewModel) {
                    ContentFolderWrapper folder = folderViewModel.Folder;
                    if (CutFlag == ClipboardController.CutFlagEnum.Folder) {
                        // Cutフラグが立っている場合はコピー元のフォルダを削除する
                        folder.MoveToAsync(toFolder.Folder);
                        // 元のフォルダの親フォルダを再読み込み
                        folderViewModel.ParentFolderViewModel?.FolderCommands.LoadFolderCommand.Execute();
                    }
                }

            }
            toFolder.FolderCommands.LoadFolderCommand.Execute();

            LogWrapper.Info(CommonStringResources.Instance.Pasted);
        }

    }
}
