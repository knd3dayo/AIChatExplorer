using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Folders.Browser;
using AIChatExplorer.ViewModel.Folders.Chat;
using AIChatExplorer.ViewModel.Folders.FileSystem;
using AIChatExplorer.ViewModel.Folders.Mail;
using AIChatExplorer.ViewModel.Folders.ScreenShot;
using AIChatExplorer.ViewModel.Folders.Search;
using AIChatExplorer.ViewModel.Folders.ShortCut;
using LibMain.Model.Content;
using LibMain.Model.Folders;
using LibUIMain.ViewModel.Common;
using LibUIMain.ViewModel.Folder;
using LibUIMain.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Main {
    public class FolderViewModelManager : FolderViewModelManagerBase {

        public override ContentFolderViewModel? GetApplicationRootFolderViewModel() {
            return RootFolderViewModel;
        }
        public override ContentFolderViewModel? GetSearchRootFolderViewModel() {
            return SearchRootFolderViewModel;
        }

        public override async Task<ContentFolderViewModel?> CreateFolderViewModel(string folderId, string type) {
            // itemのFolderTypeStringに応じてContentFolderViewModelを作成する

            if (type == FolderManager.APPLICATION_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.CHAT_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ChatHistoryFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.FILESYSTEM_ROOT_FOLDER_NAME_EN) {
                FileSystemFolder? fileSystemFolder = await ContentFolderWrapper.GetFolderById<FileSystemFolder>(folderId);
                if (fileSystemFolder != null) {
                    return new FileSystemFolderViewModel(fileSystemFolder, Commands);
                }
            } else if (type == FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN) {
                FileSystemFolder? fileSystemFolder = await ContentFolderWrapper.GetFolderById<FileSystemFolder>(folderId);
                if (fileSystemFolder != null) {
                    return new ShortCutFolderViewModel(fileSystemFolder, Commands);
                }
            } else if (type == FolderManager.RECENT_FILES_ROOT_FOLDER_NAME_EN) {
                FileSystemFolder? fileSystemFolder = await ContentFolderWrapper.GetFolderById<FileSystemFolder>(folderId);
                if (fileSystemFolder != null) {
                    return new RecentFilesFolderViewModel(fileSystemFolder, Commands);
                }
            } else if (type == FolderManager.EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new EdgeBrowseHistoryFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN) {
                ApplicationFolder? item = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderId);
                if (item != null) {
                    return new ApplicationFolderViewModel(item, Commands);
                }
            } else if (type == FolderManager.OUTLOOK_ROOT_FOLDER_NAME_EN && OutlookFolder.OutlookApplicationExists()) {
                OutlookFolder? outlookFolder = await ContentFolderWrapper.GetFolderById<OutlookFolder>(folderId);
                if (outlookFolder != null) {
                    return new OutlookFolderViewModel(outlookFolder, Commands);
                }
            } else if (type == FolderManager.SEARCH_ROOT_FOLDER_NAME_EN) {
                SearchFolder? searchFolder = await ContentFolderWrapper.GetFolderById<SearchFolder>(folderId);
                if (searchFolder != null) {
                    return new SearchFolderViewModel(searchFolder, Commands);
                }
            }
            return null;
        }

        public override async Task<ContentItemViewModel?> CreateItemViewModel(ContentItem item) {
            // ConentFolderWrapperを取得
            ContentFolderWrapper? folder = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(item.FolderId);
            if (folder == null) {
                return null;
            }
            var folderType = folder.FolderTypeString;
            var folderViewModel = await CreateFolderViewModel(folder.Id, folderType);
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
        private ApplicationFolderViewModel? RootFolderViewModel { get; set; }

        // 検索フォルダのClipboardViewModel
        private SearchFolderViewModel? SearchRootFolderViewModel { get; set; }

        // チャットフォルダのClipboardViewModel
        public ChatHistoryFolderViewModel? ChatRootFolderViewModel { get; private set; }

        // ローカルファイルシステムのフォルダのViewModel
        public FileSystemFolderViewModel? FileSystemFolderViewModel { get; private set; }

        // ローカルファイルシステムのショートカットのViewModel
        public ShortCutFolderViewModel? ShortcutFolderViewModel { get; private set; }

        // RecentFilesフォルダのViewModel
        public RecentFilesFolderViewModel? RecentFilesFolderViewModel { get; private set; }

        // EdgeBrowseHistoryフォルダのViewModel
        public EdgeBrowseHistoryFolderViewModel? EdgeBrowseHistoryFolderViewModel { get; private set; }

        // OutlookフォルダのViewModel
        public OutlookFolderViewModel? OutlookFolderViewModel { get; private set; }

        // ClipboardHistoryフォルダのViewModel
        public ApplicationFolderViewModel? ClipboardHistoryFolderViewModel { get; private set; }

        // ScreenShotHistoryフォルダのViewModel
        public ScreenShotHistoryFolderViewModel? ScreenShotHistoryFolderViewModel { get; private set; }

        
        // コンストラクタ
        public FolderViewModelManager(CommonViewModelCommandExecutes commands) : base(commands) {
            var applicationRootFolder = FolderManager.GetApplicationRootFolder();
            var searchRootFolder = FolderManager.GetSearchRootFolder();
            var chatRootFolder = FolderManager.GetChatRootFolder();
            var fileSystemRootFolder = FolderManager.GetFileSystemRootFolder();
            var shortcutRootFolder = FolderManager.GetShortcutRootFolder();
            var recentFilesRootFolder = FolderManager.GetRecentFilesRootFolder();
            var edgeBrowseHistoryRootFolder = FolderManager.GetEdgeBrowseHistoryRootFolder();
            var clipboardHistoryRootFolder = FolderManager.GetClipboardHistoryRootFolder();
            var screenShotHistoryRootFolder = FolderManager.GetScreenShotHistoryRootFolder();
            var integratedMonitorHistoryRootFolder = FolderManager.GetIntegratedMonitorHistoryRootFolder();
            var outlookRootFolder = FolderManager.GetOutlookRootFolder();

            RootFolderViewModel = new ApplicationFolderViewModel(applicationRootFolder, commands);
            SearchRootFolderViewModel = new SearchFolderViewModel(searchRootFolder, commands);
            ChatRootFolderViewModel = new ChatHistoryFolderViewModel(chatRootFolder, commands);
            FileSystemFolderViewModel = new FileSystemFolderViewModel(fileSystemRootFolder, MainWindowViewModel.Instance.Commands);
            ShortcutFolderViewModel = new ShortCutFolderViewModel(shortcutRootFolder, commands);
            RecentFilesFolderViewModel = new RecentFilesFolderViewModel(recentFilesRootFolder, commands);
            EdgeBrowseHistoryFolderViewModel = new EdgeBrowseHistoryFolderViewModel(edgeBrowseHistoryRootFolder, commands);
            ClipboardHistoryFolderViewModel = new ApplicationFolderViewModel(clipboardHistoryRootFolder, commands);
            ScreenShotHistoryFolderViewModel = new ScreenShotHistoryFolderViewModel(screenShotHistoryRootFolder, commands);
            if (OutlookFolder.OutlookApplicationExists()) {
                OutlookFolderViewModel = new OutlookFolderViewModel(outlookRootFolder, MainWindowViewModel.Instance.Commands);
            }
            FolderViewModels.Clear();
            FolderViewModels.Add(RootFolderViewModel);
            FolderViewModels.Add(FileSystemFolderViewModel);
            FolderViewModels.Add(ShortcutFolderViewModel);
            FolderViewModels.Add(RecentFilesFolderViewModel);
            FolderViewModels.Add(EdgeBrowseHistoryFolderViewModel);
            if (OutlookFolderViewModel != null) {
                FolderViewModels.Add(OutlookFolderViewModel);
            }

            FolderViewModels.Add(SearchRootFolderViewModel);
            FolderViewModels.Add(ChatRootFolderViewModel);
            FolderViewModels.Add(ClipboardHistoryFolderViewModel);
            FolderViewModels.Add(ScreenShotHistoryFolderViewModel);

            OnPropertyChanged(nameof(FolderViewModels));

        }

    }
}
