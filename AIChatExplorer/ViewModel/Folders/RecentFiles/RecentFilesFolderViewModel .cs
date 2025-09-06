using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.ViewModel.Folders.FileSystem;
using AIChatExplorer.ViewModel.Folders.ShortCut;
using LibPythonAI.Model.Content;
using LibUIMain.ViewModel.Common;
using LibUIMain.ViewModel.Folder;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class RecentFilesFolderViewModel(FileSystemFolder applicationItemFolder, CommonViewModelCommandExecutes commands) : FileSystemFolderViewModel(applicationItemFolder, commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ShortCutFolderMenu applicationItemMenu = new(this);
                return applicationItemMenu.MenuItems;
            }
        }

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override RecentFilesFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            if (childFolder is not RecentFilesFolder) {
                throw new Exception("childFolder is not RecentFilesFolder");
            }
            var childFolderViewModel = new RecentFilesFolderViewModel((RecentFilesFolder)childFolder, commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadLLMConfigListAsync
        public override async Task LoadItemsAsync() {
           await LoadItemsAsync<RecentFilesItem>();
        }

        // LoadChildren
        public override async Task LoadChildren(int nestLevel) {
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<RecentFilesFolderViewModel> _children = [];

            if (Folder.IsRootFolder) {
                var childFolders = await Folder.GetChildrenAsync<RecentFilesFolder>(true);
                foreach (var child in childFolders) {
                    if (child == null) continue;
                    RecentFilesFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                    _children.Add(childViewModel);
                }
            } else {
                var childFolders = await Folder.GetChildrenAsync<FileSystemFolder>(true);
                foreach (var child in childFolders) {
                    if (child == null) continue;
                    RecentFilesFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                    if (nestLevel > 0) {
                        await childViewModel.LoadChildren(nestLevel - 1);
                    }
                    _children.Add(childViewModel);
                }
            }
            Children = new ObservableCollection<ContentFolderViewModel>(_children);
            OnPropertyChanged(nameof(Children));
        }
    }
}

