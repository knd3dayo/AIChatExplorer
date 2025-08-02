using AIChatExplorer.Model.Folders.Outlook;
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
using LibPythonAI.Model.Content;
using LibUIPythonAI.ViewModel.Item;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.ViewModel.Content;
using LibPythonAI.Model.Folders;

namespace AIChatExplorer.ViewModel.Main {
    public class FolderViewModelManager : FolderViewModelManagerBase {

        public override ContentFolderViewModel GetApplicationRootFolderViewModel() {
            return RootFolderViewModel;
        }
        public override ContentFolderViewModel GetSearchRootFolderViewModel() {
            return SearchRootFolderViewModel;
        }

        public override ContentFolderViewModel? CreateFolderViewModel(string folderId, string type) {
            // itemのFolderTypeStringに応じてContentFolderViewModelを作成する

            if (type == FolderManager.APPLICATION_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.CHAT_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ChatHistoryFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.FILESYSTEM_ROOT_FOLDER_NAME_EN) {
                FileSystemFolder? fileSystemFolder = ContentFolderWrapper.GetFolderById<FileSystemFolder>(folderId);
                if (fileSystemFolder != null) {
                    return new FileSystemFolderViewModel(fileSystemFolder, Commands);
                }
            } else if (type == FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN) {
                FileSystemFolder? fileSystemFolder = ContentFolderWrapper.GetFolderById<FileSystemFolder>(folderId);
                if (fileSystemFolder != null) {
                    return new ShortCutFolderViewModel(fileSystemFolder, Commands);
                }
            } else if (type == FolderManager.RECENT_FILES_ROOT_FOLDER_NAME_EN) {
                FileSystemFolder? fileSystemFolder = ContentFolderWrapper.GetFolderById<FileSystemFolder>(folderId);
                if (fileSystemFolder != null) {
                    return new RecentFilesFolderViewModel(fileSystemFolder, Commands);
                }
            } else if (type == FolderManager.EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new EdgeBrowseHistoryFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
                return new ApplicationFolderViewModel(item, Commands);
            } else if (type == FolderManager.OUTLOOK_ROOT_FOLDER_NAME_EN && OutlookFolder.OutlookApplicationExists()) {
                OutlookFolder? outlookFolder = ContentFolderWrapper.GetFolderById<OutlookFolder>(folderId);
                if (outlookFolder != null) {
                    return new OutlookFolderViewModel(outlookFolder, Commands);
                }
            } else if (type == FolderManager.SEARCH_ROOT_FOLDER_NAME_EN) {
                SearchFolder? searchFolder = ContentFolderWrapper.GetFolderById<SearchFolder>(folderId);
                if (searchFolder != null) {
                    return new SearchFolderViewModel(searchFolder, Commands);
                }
            }
            return null;
        }

        public override ContentItemViewModel? CreateItemViewModel(ContentItemWrapper item) {
            // ConentFolderWrapperを取得
            ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById<ApplicationFolder>(item.FolderId);
            if (folder == null) {
                return null;
            }
            var folderType = folder.FolderTypeString;
            var folderViewModel = CreateFolderViewModel(folder.Id, folderType);
            if (folderViewModel == null) {
                return null;
            }
            // アイテムのViewModelを作成
            if (folderType == FolderManager.APPLICATION_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.CHAT_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.FILESYSTEM_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.RECENT_FILES_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN) {
                return new EdgeBrowseHistoryItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.OUTLOOK_ROOT_FOLDER_NAME_EN && OutlookFolder.OutlookApplicationExists()) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else if (folderType == FolderManager.SEARCH_ROOT_FOLDER_NAME_EN) {
                return new ApplicationItemViewModel(folderViewModel, item);
            } else {
                return null;
            }
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
