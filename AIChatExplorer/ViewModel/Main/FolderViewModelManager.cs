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
using AIChatExplorer.ViewModel.Folders.ScreenShot;

namespace AIChatExplorer.ViewModel.Main {
    public class FolderViewModelManager : FolderViewModelManagerBase {

        public override ContentFolderViewModel GetApplicationRootFolderViewModel() {
            return RootFolderViewModel;
        }
        public override ContentFolderViewModel GetSearchRootFolderViewModel() {
            return SearchRootFolderViewModel;
        }

        // RootFolderのClipboardViewModel
        private ApplicationFolderViewModel RootFolderViewModel { get; set; }

        // 検索フォルダのClipboardViewModel
        private SearchFolderViewModel SearchRootFolderViewModel { get; set; }

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

        // ScreenShotHistoryフォルダのViewModel
        public ScreenShotHistoryFolderViewModel ScreenShotHistoryFolderViewModel { get; private set; }

        // IntegratedMonitorHistoryフォルダのViewModel
        public IntegratedMonitorHistoryFolderViewModel IntegratedMonitorHistoryFolderViewModel { get; private set; }

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
            ScreenShotHistoryFolderViewModel = new ScreenShotHistoryFolderViewModel(FolderManager.ScreenShotHistoryRootFolder, commands);
            IntegratedMonitorHistoryFolderViewModel = new IntegratedMonitorHistoryFolderViewModel(FolderManager.IntegratedMonitorHistoryRootFolder, commands);


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
            FolderViewModels.Add(ScreenShotHistoryFolderViewModel);
            FolderViewModels.Add(IntegratedMonitorHistoryFolderViewModel);

            OnPropertyChanged(nameof(FolderViewModels));
        }

    }
}
