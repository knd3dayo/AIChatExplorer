using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.Model.Main;
using AIChatExplorer.ViewModel.Folders.Browser;
using AIChatExplorer.ViewModel.Folders.Chat;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Folders.FileSystem;
using AIChatExplorer.ViewModel.Folders.Mail;
using AIChatExplorer.ViewModel.Folders.Search;
using AIChatExplorer.ViewModel.Folders.ShortCut;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Common;

namespace AIChatExplorer.ViewModel.Main {
    public class FolderViewModelManager : RootFolderViewModelContainer {

        // RootFolderのClipboardViewModel
        public ApplicationFolderViewModel RootFolderViewModel { get; private set; }

        // 検索フォルダのClipboardViewModel
        public SearchFolderViewModel SearchRootFolderViewModel { get; private set; }

        // チャットフォルダのClipboardViewModel
        public ChatHistoryFolderViewModel ChatRootFolderViewModel { get; private set; }

        // ローカルファイルシステムのフォルダのViewModel
        public FileSystemFolderViewModel FileSystemFolderViewModel { get; private set; }

        // ローカルファイルシステムのショートカットのViewModel
        public ShortCutFolderViewModel ShortcutFolderViewModel { get; private set; }

        // RecentFilesフォルダのViewModel
        public RecentFilesFolderViewModel RecentFilesFolderViewModel { get; private set; }

        // EdgeBrowseHistoryフォルダのViewModel
        public EdgeBrowseHistoryFolderViewModel EdgeBrowseHistoryFolderViewModel { get; private set; }

        // OutlookフォルダのViewModel
        public OutlookFolderViewModel? OutlookFolderViewModel { get; private set; }

        // ClipboardHistoryフォルダのViewModel
        public ApplicationFolderViewModel ClipboardHistoryFolderViewModel { get; private set; }

        // コンストラクタ
        public FolderViewModelManager(CommonViewModelCommandExecutes commands) : base(commands) {
            RootFolderViewModel = new ApplicationFolderViewModel(FolderManager.RootFolder, commands);
            SearchRootFolderViewModel = new SearchFolderViewModel(FolderManager.SearchRootFolder, commands);
            ChatRootFolderViewModel = new ChatHistoryFolderViewModel(FolderManager.ChatRootFolder, commands);
            FileSystemFolderViewModel = new FileSystemFolderViewModel(FolderManager.FileSystemRootFolder, MainWindowViewModel.Instance.Commands);
            ShortcutFolderViewModel = new ShortCutFolderViewModel(FolderManager.ShortcutRootFolder, commands);
            RecentFilesFolderViewModel = new RecentFilesFolderViewModel(FolderManager.RecentFilesRootFolder, commands);
            EdgeBrowseHistoryFolderViewModel = new EdgeBrowseHistoryFolderViewModel(FolderManager.EdgeBrowseHistoryRootFolder, commands);
            ClipboardHistoryFolderViewModel = new ApplicationFolderViewModel(FolderManager.ClipboardHistoryRootFolder, commands);

            FolderViewModels.Clear();
            FolderViewModels.Add(RootFolderViewModel);
            FolderViewModels.Add(FileSystemFolderViewModel);
            FolderViewModels.Add(ShortcutFolderViewModel);
            FolderViewModels.Add(RecentFilesFolderViewModel);
            FolderViewModels.Add(EdgeBrowseHistoryFolderViewModel);
            if (OutlookFolder.OutlookApplicationExists()) {
                OutlookFolderViewModel = new OutlookFolderViewModel(FolderManager.OutlookRootFolder, MainWindowViewModel.Instance.Commands);
                FolderViewModels.Add(OutlookFolderViewModel);
            }

            FolderViewModels.Add(SearchRootFolderViewModel);
            FolderViewModels.Add(ChatRootFolderViewModel);
            FolderViewModels.Add(ClipboardHistoryFolderViewModel);

            OnPropertyChanged(nameof(FolderViewModels));
        }

    }
}
