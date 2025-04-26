using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.ViewModel.Folders.Clipboard;
using AIChatExplorer.ViewModel.Main;
using LibPythonAI.Model.Content;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryFolderViewModel(ContentFolderWrapper clipboardItemFolder, ContentItemViewModelCommands commands) : ClipboardFolderViewModel(clipboardItemFolder, commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                EdgeBrowseHistoryFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }
        // RootFolderのViewModelを取得する
        public override ContentFolderViewModel GetRootFolderViewModel() {
            return MainWindowViewModel.Instance.RootFolderViewModelContainer.EdgeBrowseHistoryFolderViewModel;
        }

        public override EdgeBrowseHistoryItemViewModel CreateItemViewModel(ContentItemWrapper item) {
            return new EdgeBrowseHistoryItemViewModel(this, item);
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override EdgeBrowseHistoryFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var childFolderViewModel = new EdgeBrowseHistoryFolderViewModel(childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadItems
        public override void LoadItems() {
            LoadItems<EdgeBrowseHistoryItem>();
        }

        // LoadChildren
        public override void LoadChildren(int nestLevel) {
            LoadChildren<EdgeBrowseHistoryFolderViewModel, EdgeBrowseHistoryFolder>(nestLevel);
        }
    }
}

