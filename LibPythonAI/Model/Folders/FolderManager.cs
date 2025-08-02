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
        }

        //--------------------------------------------------------------------------------
        private static ApplicationFolder? applicationRootFolder;
        public static ApplicationFolder RootFolder {
            get {

                if (applicationRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == APPLICATION_ROOT_FOLDER_NAME_EN);
                    ApplicationFolder? folder = ContentFolderWrapper.GetFolderById<ApplicationFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = APPLICATION_ROOT_FOLDER_NAME,
                            FolderTypeString = APPLICATION_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }
                    applicationRootFolder = folder;

                }
                return applicationRootFolder;
            }
        }
        private static SearchFolder? searchRootFolder;
        public static SearchFolder SearchRootFolder {
            get {
                if (searchRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == SEARCH_ROOT_FOLDER_NAME_EN);
                    SearchFolder? folder = ContentFolderWrapper.GetFolderById<SearchFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = SEARCH_ROOT_FOLDER_NAME,
                            FolderTypeString = SEARCH_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }
                    searchRootFolder = folder;

                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (searchRootFolder.IsRootFolder == false) {
                    searchRootFolder.IsRootFolder = true;
                    searchRootFolder.Save();
                }
                return searchRootFolder;
            }
        }

        // Local File System Root Folder
        private static FileSystemFolder? fileSystemRootFolder;
        public static FileSystemFolder FileSystemRootFolder {
            get {
                if (fileSystemRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == FILESYSTEM_ROOT_FOLDER_NAME_EN);
                    FileSystemFolder? folder = ContentFolderWrapper.GetFolderById<FileSystemFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = FILESYSTEM_ROOT_FOLDER_NAME,
                            FolderTypeString = FILESYSTEM_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }

                    fileSystemRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (fileSystemRootFolder.IsRootFolder == false) {
                    fileSystemRootFolder.IsRootFolder = true;
                    fileSystemRootFolder.Save();
                }
                return fileSystemRootFolder;
            }
        }
        // Shortcut Root Folder
        private static ShortCutFolder? shortcutRootFolder;
        public static ShortCutFolder ShortcutRootFolder {
            get {
                if (shortcutRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == SHORTCUT_ROOT_FOLDER_NAME_EN);
                    ShortCutFolder? folder = ContentFolderWrapper.GetFolderById<ShortCutFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = SHORTCUT_ROOT_FOLDER_NAME,
                            FolderTypeString = SHORTCUT_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }

                    shortcutRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (shortcutRootFolder.IsRootFolder == false) {
                    shortcutRootFolder.IsRootFolder = true;
                    shortcutRootFolder.Save();
                }
                return shortcutRootFolder;
            }
        }
        // Outlook Root Folder
        private static OutlookFolder? outlookRootFolder;
        public static OutlookFolder OutlookRootFolder {
            get {
                if (outlookRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == OUTLOOK_ROOT_FOLDER_NAME_EN);
                    OutlookFolder? folder = ContentFolderWrapper.GetFolderById<OutlookFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = OUTLOOK_ROOT_FOLDER_NAME,
                            FolderTypeString = OUTLOOK_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }

                    outlookRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (outlookRootFolder.IsRootFolder == false) {
                    outlookRootFolder.IsRootFolder = true;
                    outlookRootFolder.Save();
                }
                return outlookRootFolder;
            }

        }
        // EdgeBrowseHistory Root Folder
        private static EdgeBrowseHistoryFolder? edgeBrowseHistoryRootFolder;

        public static EdgeBrowseHistoryFolder EdgeBrowseHistoryRootFolder {
            get {
                if (edgeBrowseHistoryRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN);
                    EdgeBrowseHistoryFolder? folder = ContentFolderWrapper.GetFolderById<EdgeBrowseHistoryFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME,
                            FolderTypeString = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }

                    edgeBrowseHistoryRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (edgeBrowseHistoryRootFolder.IsRootFolder == false) {
                    edgeBrowseHistoryRootFolder.IsRootFolder = true;
                    edgeBrowseHistoryRootFolder.Save();
                }
                return edgeBrowseHistoryRootFolder;
            }
        }

        // RecentFiles Root Folder
        private static RecentFilesFolder? recentFilesRootFolder;
        public static RecentFilesFolder RecentFilesRootFolder {
            get {
                if (recentFilesRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == RECENT_FILES_ROOT_FOLDER_NAME_EN);
                    RecentFilesFolder? folder = ContentFolderWrapper.GetFolderById<RecentFilesFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = RECENT_FILES_ROOT_FOLDER_NAME,
                            FolderTypeString = RECENT_FILES_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }
                    recentFilesRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (recentFilesRootFolder.IsRootFolder == false) {
                    recentFilesRootFolder.IsRootFolder = true;
                    recentFilesRootFolder.Save();
                }

                return recentFilesRootFolder;
            }
        }
        // ClipboardHistory Root Folder
        private static ClipboardHistoryFolder? clipboardHistoryRootFolder;
        public static ClipboardHistoryFolder ClipboardHistoryRootFolder {
            get {
                if (clipboardHistoryRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN);
                    ClipboardHistoryFolder? folder = ContentFolderWrapper.GetFolderById<ClipboardHistoryFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = CLIPBOARD_HISTORY_ROOT_FOLDER_NAME,
                            FolderTypeString = CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }
                    clipboardHistoryRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (clipboardHistoryRootFolder.IsRootFolder == false) {
                    clipboardHistoryRootFolder.IsRootFolder = true;
                    clipboardHistoryRootFolder.Save();
                }
                return clipboardHistoryRootFolder;
            }

        }
        //--------------------------------------------------------------------------------
        private static ContentFolderWrapper? chatRootFolder;

        public static ContentFolderWrapper ChatRootFolder {
            get {
                if (chatRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == CHAT_ROOT_FOLDER_NAME_EN);
                    ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = CHAT_ROOT_FOLDER_NAME,
                            FolderTypeString = CHAT_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }
                    chatRootFolder = folder;
                }
                return chatRootFolder;
            }
        }

        // ScreenShotHistory Root Folder
        private static ScreenShotHistoryFolder? screenShotHistoryRootFolder;
        public static ScreenShotHistoryFolder ScreenShotHistoryRootFolder {
            get {
                if (screenShotHistoryRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN);
                    ScreenShotHistoryFolder? folder = ContentFolderWrapper.GetFolderById<ScreenShotHistoryFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME,
                            FolderTypeString = SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }
                    screenShotHistoryRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (screenShotHistoryRootFolder.IsRootFolder == false) {
                    screenShotHistoryRootFolder.IsRootFolder = true;
                    screenShotHistoryRootFolder.Save();
                }
                return screenShotHistoryRootFolder;
            }
        }
        // IntegratedMonitorHistory Root Folder
        private static IntegratedMonitorHistoryFolder? integratedMonitorHistoryRootFolder;
        public static IntegratedMonitorHistoryFolder IntegratedMonitorHistoryRootFolder {
            get {
                if (integratedMonitorHistoryRootFolder == null) {
                    var entity = RootFolderEntities.FirstOrDefault(x => x.FolderTypeString == INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN);
                    IntegratedMonitorHistoryFolder? folder = ContentFolderWrapper.GetFolderById<IntegratedMonitorHistoryFolder>(entity?.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME,
                            FolderTypeString = INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }
                    integratedMonitorHistoryRootFolder = folder;
                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (integratedMonitorHistoryRootFolder.IsRootFolder == false) {
                    integratedMonitorHistoryRootFolder.IsRootFolder = true;
                    integratedMonitorHistoryRootFolder.Save();
                }
                return integratedMonitorHistoryRootFolder;
            }
        }
    }
}