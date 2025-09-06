using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.ViewModel.Folders.Application;
using LibPythonAI.Model.Content;
using LibUIMain.ViewModel.Common;

namespace AIChatExplorer.ViewModel.Folders.Mail {
    public class OutlookFolderViewModel(ContentFolderWrapper applicationItemFolder, CommonViewModelCommandExecutes commands) : ApplicationFolderViewModel(applicationItemFolder, commands) {
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
            var childFolderViewModel = new OutlookFolderViewModel(childFolder, commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }


        // LoadLLMConfigListAsync
        public override async Task LoadItemsAsync() {
            await LoadItemsAsync<OutlookItem>();
        }

        // LoadChildren
        public override async Task LoadChildren(int nestLevel) {
            await LoadChildren<OutlookFolderViewModel, OutlookFolder>(nestLevel);
        }

    }
}

