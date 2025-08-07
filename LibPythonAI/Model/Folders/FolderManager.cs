using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.Model.Folders.ScreenShot;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.Model.Folders.ShortCut;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;

namespace LibPythonAI.Model.Folders {
    public class FolderManager {

        public static readonly string APPLICATION_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.Application;
        public static readonly string SEARCH_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.SearchFolder;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.ImageChat;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.FileSystem;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.Shortcut;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.Outlook;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.EdgeBrowseHistory;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.RecentFiles;
        public static readonly string CHAT_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.ChatHistory;
        public static readonly string CLIPBOARD_HISTORY_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.ClipboardHistory;
        public static readonly string SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.ScreenShotHistory;
        public static readonly string INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.IntegratedMonitorHistory;


        // 英語名
        public static readonly string APPLICATION_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ApplicationEnglish;
        public static readonly string SEARCH_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.SearchFolderEnglish;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ImageChatEnglish;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.FileSystemEnglish;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ShortcutEnglish;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.OutlookEnglish;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.EdgeBrowseHistoryEnglish;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.RecentFilesEnglish;
        public static readonly string CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ClipboardHistoryEnglish;
        public static readonly string CHAT_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ChatHistoryEnglish;
        public static readonly string SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ScreenShotHistoryEnglish;
        public static readonly string INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.IntegratedMonitorHistoryEnglish;


        // 言語変更時にルートフォルダ名を変更する
        public static async Task ChangeRootFolderNames(PythonAILibStringResources toRes) {
            List<ContentFolderRequest> rootFolderRequests = [];
            // ClipboardRootFolder

            var applicationRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == APPLICATION_ROOT_FOLDER_NAME_EN);

            if (applicationRootFolder != null) {
                applicationRootFolder.FolderName = toRes.Application;
                rootFolderRequests.Add(new ContentFolderRequest(applicationRootFolder));
            }
            // SearchRootFolder
            var searchRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == SEARCH_ROOT_FOLDER_NAME_EN);
            if (searchRootFolder != null) {
                searchRootFolder.FolderName = toRes.SearchFolder;
                rootFolderRequests.Add(new ContentFolderRequest(searchRootFolder));
            }
            // ChatRootFolder
            var chatRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == CHAT_ROOT_FOLDER_NAME_EN);
            if (chatRootFolder != null) {
                chatRootFolder.FolderName = toRes.ChatHistory;
                rootFolderRequests.Add(new ContentFolderRequest(chatRootFolder));
            }
            // ImageCheckRootFolder
            var imageCheckRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == IMAGECHECK_ROOT_FOLDER_NAME_EN);
            if (imageCheckRootFolder != null) {
                imageCheckRootFolder.FolderName = toRes.ImageChat;
                rootFolderRequests.Add(new ContentFolderRequest(imageCheckRootFolder));
            }
            // FileSystemRootFolder
            var fileSystemRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == FILESYSTEM_ROOT_FOLDER_NAME_EN);
            if (fileSystemRootFolder != null) {
                fileSystemRootFolder.FolderName = toRes.FileSystem;
                rootFolderRequests.Add(new ContentFolderRequest(fileSystemRootFolder));
            }
            // ShortcutRootFolder
            var shortcutRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == SHORTCUT_ROOT_FOLDER_NAME_EN);
            if (shortcutRootFolder != null) {
                shortcutRootFolder.FolderName = toRes.Shortcut;
                rootFolderRequests.Add(new ContentFolderRequest(shortcutRootFolder));
            }
            // EdgeBrowseHistoryRootFolder
            var edgeBrowseHistoryRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN);
            if (edgeBrowseHistoryRootFolder != null) {
                edgeBrowseHistoryRootFolder.FolderName = toRes.EdgeBrowseHistory;
                rootFolderRequests.Add(new ContentFolderRequest(edgeBrowseHistoryRootFolder));
            }
            // RecentFilesRootFolder
            var recentFilesRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == RECENT_FILES_ROOT_FOLDER_NAME_EN);
            if (recentFilesRootFolder != null) {
                recentFilesRootFolder.FolderName = toRes.RecentFiles;
                rootFolderRequests.Add(new ContentFolderRequest(recentFilesRootFolder));
            }
            // ClipboardHistoryRootFolder
            var clipboardHistoryRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN);
            if (clipboardHistoryRootFolder != null) {
                clipboardHistoryRootFolder.FolderName = toRes.ClipboardHistory;
                rootFolderRequests.Add(new ContentFolderRequest(clipboardHistoryRootFolder));
            }

            // ScreenShotHistoryRootFolder
            var screenShotHistoryRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN);
            if (screenShotHistoryRootFolder != null) {
                screenShotHistoryRootFolder.FolderName = toRes.ScreenShotHistory;
                rootFolderRequests.Add(new ContentFolderRequest(screenShotHistoryRootFolder));
            }
            // IntegratedMonitorHistoryRootFolder
            var integratedMonitorHistoryRootFolder = RootFolderEntities.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN);
            if (integratedMonitorHistoryRootFolder != null) {
                integratedMonitorHistoryRootFolder.FolderName = toRes.IntegratedMonitorHistory;
                rootFolderRequests.Add(new ContentFolderRequest(integratedMonitorHistoryRootFolder));
            }
            await PythonExecutor.PythonAIFunctions.UpdateContentFoldersAsync(rootFolderRequests);
        }

        private static List<ContentFolderEntity> RootFolderEntities = [];
        public static async Task InitAsync() {
            RootFolderEntities = await PythonExecutor.PythonAIFunctions.GetRootContentFoldersAsync();

            //追加設定.言語の設定
            await FolderManager.ChangeRootFolderNames(PythonAILibStringResources.Instance);

            // ApplicationRootFolder
            var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == APPLICATION_ROOT_FOLDER_NAME_EN);
            applicationRootFolder = await ContentFolderWrapper.GetFolderById<ApplicationFolder>(entity?.Id);
            // SearchFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == SEARCH_ROOT_FOLDER_NAME_EN);
            searchRootFolder = await ContentFolderWrapper.GetFolderById<SearchFolder>(entity?.Id);
            // FileSystemFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == FILESYSTEM_ROOT_FOLDER_NAME_EN);
            fileSystemRootFolder = await ContentFolderWrapper.GetFolderById<FileSystemFolder>(entity?.Id);
            // ShortCutFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == SHORTCUT_ROOT_FOLDER_NAME_EN);
            shortcutRootFolder = await ContentFolderWrapper.GetFolderById<ShortCutFolder>(entity?.Id);
            // OutlookFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == OUTLOOK_ROOT_FOLDER_NAME_EN);
            outlookRootFolder = await ContentFolderWrapper.GetFolderById<OutlookFolder>(entity?.Id);
            // EdgeBrowseHistoryFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN);
            edgeBrowseHistoryRootFolder = await ContentFolderWrapper.GetFolderById<EdgeBrowseHistoryFolder>(entity?.Id);
            // RecentFilesFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == RECENT_FILES_ROOT_FOLDER_NAME_EN);
            recentFilesRootFolder = await ContentFolderWrapper.GetFolderById<RecentFilesFolder>(entity?.Id);
            // ClipboardHistoryFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN);
            clipboardHistoryRootFolder = await ContentFolderWrapper.GetFolderById<ClipboardHistoryFolder>(entity?.Id);
            // ContentFolderWrapper
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == CHAT_ROOT_FOLDER_NAME_EN);
            chatRootFolder = await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(entity?.Id);
            // ScreenShotHistoryFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN);
            screenShotHistoryRootFolder = await ContentFolderWrapper.GetFolderById<ScreenShotHistoryFolder>(entity?.Id);
            // IntegratedMonitorHistoryFolder
            entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN);
            integratedMonitorHistoryRootFolder = await ContentFolderWrapper.GetFolderById<IntegratedMonitorHistoryFolder>(entity?.Id);


        }

        //--------------------------------------------------------------------------------
        private static ApplicationFolder? applicationRootFolder;
        public static ApplicationFolder GetApplicationRootFolder() {
            if (applicationRootFolder == null) {
                ApplicationFolder folder = new() {
                    FolderName = APPLICATION_ROOT_FOLDER_NAME,
                    FolderTypeString = APPLICATION_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                applicationRootFolder = folder;
            }

            return applicationRootFolder;
        }
        private static SearchFolder? searchRootFolder;
        public static SearchFolder GetSearchRootFolder() {
            if (searchRootFolder == null) {
                SearchFolder folder = new() {
                    FolderName = SEARCH_ROOT_FOLDER_NAME,
                    FolderTypeString = SEARCH_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run (async () => { await folder.SaveAsync(); });
                searchRootFolder = folder;

            }
            return searchRootFolder;
        }

        // Local File System Root Folder
        private static FileSystemFolder? fileSystemRootFolder;
        public static FileSystemFolder GetFileSystemRootFolder() {
            if (fileSystemRootFolder == null) {
                FileSystemFolder folder = new() {
                    FolderName = FILESYSTEM_ROOT_FOLDER_NAME,
                    FolderTypeString = FILESYSTEM_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                fileSystemRootFolder = folder;

            }
            return fileSystemRootFolder;
        }
        // Shortcut Root Folder
        private static ShortCutFolder? shortcutRootFolder;
        public static ShortCutFolder GetShortcutRootFolder() {
            if (shortcutRootFolder == null) {
                ShortCutFolder folder = new() {
                    FolderName = SHORTCUT_ROOT_FOLDER_NAME,
                    FolderTypeString = SHORTCUT_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                shortcutRootFolder = folder;

            }
            return shortcutRootFolder;
        }
        // Outlook Root Folder
        private static OutlookFolder? outlookRootFolder;
        public static OutlookFolder GetOutlookRootFolder() {
            if (outlookRootFolder == null) {
                OutlookFolder folder = new() {
                    FolderName = OUTLOOK_ROOT_FOLDER_NAME,
                    FolderTypeString = OUTLOOK_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                outlookRootFolder = folder;

            }
            return outlookRootFolder;

        }
        // EdgeBrowseHistory Root Folder
        private static EdgeBrowseHistoryFolder? edgeBrowseHistoryRootFolder;

        public static EdgeBrowseHistoryFolder GetEdgeBrowseHistoryRootFolder() {
            if (edgeBrowseHistoryRootFolder == null) {
                EdgeBrowseHistoryFolder folder = new() {
                    FolderName = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME,
                    FolderTypeString = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                edgeBrowseHistoryRootFolder = folder;
            }
            return edgeBrowseHistoryRootFolder;
        }

        // RecentFiles Root Folder
        private static RecentFilesFolder? recentFilesRootFolder;
        public static RecentFilesFolder GetRecentFilesRootFolder() {
            if (recentFilesRootFolder == null) {
                RecentFilesFolder folder = new() {
                    FolderName = RECENT_FILES_ROOT_FOLDER_NAME,
                    FolderTypeString = RECENT_FILES_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                recentFilesRootFolder = folder;
            }
            return recentFilesRootFolder;
        }

        // ClipboardHistory Root Folder
        private static ClipboardHistoryFolder? clipboardHistoryRootFolder;
        public static ClipboardHistoryFolder GetClipboardHistoryRootFolder() {
            if (clipboardHistoryRootFolder == null) {
                ClipboardHistoryFolder folder = new() {
                    FolderName = CLIPBOARD_HISTORY_ROOT_FOLDER_NAME,
                    FolderTypeString = CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                clipboardHistoryRootFolder = folder;
            }
            return clipboardHistoryRootFolder;
        }
        //--------------------------------------------------------------------------------
        private static ContentFolderWrapper? chatRootFolder;

        public static ContentFolderWrapper GetChatRootFolder() {
            if (chatRootFolder == null) {
                ContentFolderWrapper folder = new() {
                    FolderName = CHAT_ROOT_FOLDER_NAME,
                    FolderTypeString = CHAT_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                chatRootFolder = folder;
            }
            return chatRootFolder;
        }

        // ScreenShotHistory Root Folder
        private static ScreenShotHistoryFolder? screenShotHistoryRootFolder;
        public static ScreenShotHistoryFolder GetScreenShotHistoryRootFolder() {
            if (screenShotHistoryRootFolder == null) {
                ScreenShotHistoryFolder folder = new() {
                    FolderName = SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME,
                    FolderTypeString = SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                screenShotHistoryRootFolder = folder;
            }
            return screenShotHistoryRootFolder;
        }
        // IntegratedMonitorHistory Root Folder
        private static IntegratedMonitorHistoryFolder? integratedMonitorHistoryRootFolder;
        public static IntegratedMonitorHistoryFolder GetIntegratedMonitorHistoryRootFolder() {
            if (integratedMonitorHistoryRootFolder == null) {
                IntegratedMonitorHistoryFolder folder = new() {
                    FolderName = INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME,
                    FolderTypeString = INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN,
                    IsRootFolder = true,
                };
                Task.Run(async () => { await folder.SaveAsync(); });
                integratedMonitorHistoryRootFolder = folder;
            }
            return integratedMonitorHistoryRootFolder;
        }
    }
}