using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Main;
using LibPythonAI.Model.Content;
using LibUIMain.ViewModel.Common;
using LibUIMain.ViewModel.Folder;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryFolderViewModel(ContentFolderWrapper applicationItemFolder, CommonViewModelCommandExecutes Commands) : ApplicationFolderViewModel(applicationItemFolder, Commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                EdgeBrowseHistoryFolderMenu applicationItemMenu = new(this);
                return applicationItemMenu.MenuItems;
            }
        }
        // RootFolderのViewModelを取得する
        public override ContentFolderViewModel? GetRootFolderViewModel() {
            return MainWindowViewModel.Instance.RootFolderViewModelContainer.EdgeBrowseHistoryFolderViewModel;
        }

        public override EdgeBrowseHistoryItemViewModel CreateItemViewModel(ContentItem item) {
            return new EdgeBrowseHistoryItemViewModel(this, item);
        }

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override EdgeBrowseHistoryFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var childFolderViewModel = new EdgeBrowseHistoryFolderViewModel(childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadLLMConfigListAsync
        public override async Task LoadItemsAsync() {
            await LoadItemsAsync<EdgeBrowseHistoryItem>();
        }

        // LoadChildren
        public override async Task LoadChildren(int nestLevel) {
            await LoadChildren<EdgeBrowseHistoryFolderViewModel, EdgeBrowseHistoryFolder>(nestLevel);
        }
    }
}

