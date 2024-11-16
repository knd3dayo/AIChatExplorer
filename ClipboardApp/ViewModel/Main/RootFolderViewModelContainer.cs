using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using ClipboardApp.Utils;
using ClipboardApp.ViewModel.Chat;
using ClipboardApp.ViewModel.FileSystem;
using ClipboardApp.ViewModel.Search;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardApp.ViewModel.Main {
    public class RootFolderViewModelContainer: ObservableObject {

        // RootFolderのClipboardViewModel
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }

        // 検索フォルダのClipboardViewModel
        public SearchFolderViewModel SearchRootFolderViewModel { get; private set; }

        // チャットフォルダのClipboardViewModel
        public ChatFolderViewModel ChatRootFolderViewModel { get; private set; }

        // ローカルファイルシステムのフォルダのViewModel
        public FileSystemFolderViewModel FileSystemFolderViewModel { get; private set; }

        // ClipboardFolder
        public ObservableCollection<ClipboardFolderViewModel> FolderViewModels { get; set; } = [];

        // コンストラクタ
        public RootFolderViewModelContainer() {
            RootFolderViewModel = new ClipboardFolderViewModel(ClipboardFolderUtil.RootFolder);
            SearchRootFolderViewModel = new SearchFolderViewModel(ClipboardFolderUtil.SearchRootFolder);
            ChatRootFolderViewModel = new ChatFolderViewModel(ClipboardFolderUtil.ChatRootFolder);
            FileSystemFolderViewModel = new FileSystemFolderViewModel(ClipboardFolderUtil.FileSystemRootFolder);
            FolderViewModels.Add(RootFolderViewModel);
            FolderViewModels.Add(FileSystemFolderViewModel);
            FolderViewModels.Add(SearchRootFolderViewModel);
            FolderViewModels.Add(ChatRootFolderViewModel);

            OnPropertyChanged(nameof(FolderViewModels));
        }

    }
}
