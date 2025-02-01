using ClipboardApp.Model.Folder;
using ClipboardApp.ViewModel.Folders.Chat;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.FileSystem;
using ClipboardApp.ViewModel.Folders.Mail;
using ClipboardApp.ViewModel.Folders.Search;
using ClipboardApp.ViewModel.Folders.ShortCut;
using PythonAILibUI.ViewModel.Folder;

namespace ClipboardApp.ViewModel.Main {
    public class ClipboardAppRootFolderViewModelContainer : RootFolderViewModelContainer {

        // RootFolderのClipboardViewModel
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }

        // 検索フォルダのClipboardViewModel
        public SearchFolderViewModel SearchRootFolderViewModel { get; private set; }

        // チャットフォルダのClipboardViewModel
        public ChatFolderViewModel ChatRootFolderViewModel { get; private set; }

        // ローカルファイルシステムのフォルダのViewModel
        public FileSystemFolderViewModel FileSystemFolderViewModel { get; private set; }

        // ローカルファイルシステムのショートカットのViewModel
        public ShortCutFolderViewModel ShortcutFolderViewModel { get; private set; }

        // OutlookフォルダのViewModel
        public OutlookFolderViewModel? OutlookFolderViewModel { get; private set; }


        // コンストラクタ
        public ClipboardAppRootFolderViewModelContainer() {
            RootFolderViewModel = new ClipboardFolderViewModel(FolderManager.RootFolder);
            SearchRootFolderViewModel = new SearchFolderViewModel(FolderManager.SearchRootFolder);
            ChatRootFolderViewModel = new ChatFolderViewModel(FolderManager.ChatRootFolder);
            FileSystemFolderViewModel = new FileSystemFolderViewModel(FolderManager.FileSystemRootFolder);
            ShortcutFolderViewModel = new ShortCutFolderViewModel(FolderManager.ShortcutRootFolder);

            FolderViewModels.Clear();
            FolderViewModels.Add(RootFolderViewModel);
            FolderViewModels.Add(FileSystemFolderViewModel);
            FolderViewModels.Add(ShortcutFolderViewModel);
            if (OutlookFolder.OutlookApplicationExists()) {
                OutlookFolderViewModel = new OutlookFolderViewModel(FolderManager.OutlookRootFolder);
                FolderViewModels.Add(OutlookFolderViewModel);
            }

            FolderViewModels.Add(SearchRootFolderViewModel);
            FolderViewModels.Add(ChatRootFolderViewModel);

            OnPropertyChanged(nameof(FolderViewModels));
        }

    }
}
