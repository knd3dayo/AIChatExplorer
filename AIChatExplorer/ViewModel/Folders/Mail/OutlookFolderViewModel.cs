using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.Model.Item;
using AIChatExplorer.ViewModel.Folders.Application;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Mail {
    public class OutlookFolderViewModel(ContentFolderWrapper applicationItemFolder, ContentItemViewModelCommands commands) : ApplicationFolderViewModel(applicationItemFolder, commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                OutlookFolderMenu applicationItemMenu = new(this);
                return applicationItemMenu.MenuItems;
            }
        }

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override OutlookFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            if (childFolder is not OutlookFolder) {
                throw new Exception("childFolder is not OutlookFolder");
            }
            var childFolderViewModel = new OutlookFolderViewModel(childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }


        // LoadLLMConfigListAsync
        public override void LoadItems() {
            LoadItems<OutlookItem>();
        }

        // LoadChildren
        public override void LoadChildren(int nestLevel) {
            LoadChildren<OutlookFolderViewModel, OutlookFolder>(nestLevel);
        }
        public static SimpleDelegateCommand<OutlookFolderViewModel> SyncItemCommand => new(async (folderViewModel) => {
            try {
                OutlookFolder folder = (OutlookFolder)folderViewModel.Folder;
                CommonViewModelProperties.Instance.UpdateIndeterminate(true);
                await Task.Run(() => {
                    folder.SyncItems();
                });
            } finally {
                CommonViewModelProperties.Instance.UpdateIndeterminate(false);
            }
            folderViewModel.LoadItems();

        });

    }
}

