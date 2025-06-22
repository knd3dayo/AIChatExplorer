using System.IO;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.Model.Folders.ScreenShot;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.Model.Folders.ShortCut;
using LibPythonAI.Common;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Resource;

namespace AIChatExplorer.Model.Main {
    public class FolderManager : LibPythonAI.Model.Folder.FolderManager {

        public static readonly string APPLICATION_ROOT_FOLDER_NAME = CommonStringResources.Instance.Application;
        public static readonly string SEARCH_ROOT_FOLDER_NAME = CommonStringResources.Instance.SearchFolder;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME = CommonStringResources.Instance.ImageChat;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME = CommonStringResources.Instance.FileSystem;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME = CommonStringResources.Instance.Shortcut;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME = CommonStringResources.Instance.Outlook;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME = CommonStringResources.Instance.EdgeBrowseHistory;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME = CommonStringResources.Instance.RecentFiles;
        public static readonly string CLIPBOARD_HISTORY_ROOT_FOLDER_NAME = CommonStringResources.Instance.ClipboardHistory;
        public static readonly string SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME = CommonStringResources.Instance.ScreenShotHistory;
        public static readonly string INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME = CommonStringResources.Instance.IntegratedMonitorHistory;


        // 英語名
        public static readonly string APPLICATION_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ApplicationEnglish;
        public static readonly string SEARCH_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.SearchFolderEnglish;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ImageChatEnglish;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.FileSystemEnglish;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ShortcutEnglish;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.OutlookEnglish;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.EdgeBrowseHistoryEnglish;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.RecentFilesEnglish;
        public static readonly string CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ClipboardHistoryEnglish;
        public static readonly string SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ScreenShotHistoryEnglish;
        public static readonly string INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.IntegratedMonitorHistoryEnglish;


        // 言語変更時にルートフォルダ名を変更する
        public static void ChangeRootFolderNames(CommonStringResources toRes) {
            using PythonAILibDBContext db = new();
            // ClipboardRootFolder

            var applicationRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == APPLICATION_ROOT_FOLDER_NAME_EN);

            if (applicationRootFolder != null) {
                applicationRootFolder.FolderName = toRes.Application;
            }
            // SearchRootFolder
            var searchRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == SEARCH_ROOT_FOLDER_NAME_EN);
            if (searchRootFolder != null) {
                searchRootFolder.FolderName = toRes.SearchFolder;
            }
            // ChatRootFolder
            var chatRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == CHAT_ROOT_FOLDER_NAME_EN);
            if (chatRootFolder != null) {
                chatRootFolder.FolderName = toRes.ChatHistory;
            }
            // ImageCheckRootFolder
            var imageCheckRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == IMAGECHECK_ROOT_FOLDER_NAME_EN);
            if (imageCheckRootFolder != null) {
                imageCheckRootFolder.FolderName = toRes.ImageChat;
            }
            // FileSystemRootFolder
            var fileSystemRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == FILESYSTEM_ROOT_FOLDER_NAME_EN);
            if (fileSystemRootFolder != null) {
                fileSystemRootFolder.FolderName = toRes.FileSystem;
            }
            // ShortcutRootFolder
            var shortcutRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == SHORTCUT_ROOT_FOLDER_NAME_EN);
            if (shortcutRootFolder != null) {
                shortcutRootFolder.FolderName = toRes.Shortcut;
            }
            // EdgeBrowseHistoryRootFolder
            var edgeBrowseHistoryRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN);
            if (edgeBrowseHistoryRootFolder != null) {
                edgeBrowseHistoryRootFolder.FolderName = toRes.EdgeBrowseHistory;
            }
            // RecentFilesRootFolder
            var recentFilesRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == RECENT_FILES_ROOT_FOLDER_NAME_EN);
            if (recentFilesRootFolder != null) {
                recentFilesRootFolder.FolderName = toRes.RecentFiles;
            }
            // ClipboardHistoryRootFolder
            var clipboardHistoryRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN);
            if (clipboardHistoryRootFolder != null) {
                clipboardHistoryRootFolder.FolderName = toRes.ClipboardHistory;
            }

            // ScreenShotHistoryRootFolder
            var screenShotHistoryRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN);
            if (screenShotHistoryRootFolder != null) {
                screenShotHistoryRootFolder.FolderName = toRes.ScreenShotHistory;
            }
            // IntegratedMonitorHistoryRootFolder
            var integratedMonitorHistoryRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN);
            if (integratedMonitorHistoryRootFolder != null) {
                integratedMonitorHistoryRootFolder.FolderName = toRes.IntegratedMonitorHistory;
            }
            db.SaveChanges();
        }

        //--------------------------------------------------------------------------------
        private static ApplicationFolder? applicationRootFolder;
        public static ApplicationFolder RootFolder {
            get {
                if (applicationRootFolder == null) {
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(APPLICATION_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = APPLICATION_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), APPLICATION_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    ApplicationFolder? folder = ContentFolderWrapper.GetFolderById<ApplicationFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
                            FolderName = APPLICATION_ROOT_FOLDER_NAME,
                            FolderTypeString = APPLICATION_ROOT_FOLDER_NAME_EN,
                            IsRootFolder = true,
                        };
                        folder.Save();
                    }
                    applicationRootFolder = folder;

                }
                //既にルートフォルダがある環境用にIsRootFolderをtrueにする
                if (applicationRootFolder.IsRootFolder == false) {
                    applicationRootFolder.IsRootFolder = true;
                    applicationRootFolder.Save();
                }
                return applicationRootFolder;
            }
        }
        private static SearchFolder? searchRootFolder;
        public static SearchFolder SearchRootFolder {
            get {
                if (searchRootFolder == null) {
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(SEARCH_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = SEARCH_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), SEARCH_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    SearchFolder? folder = ContentFolderWrapper.GetFolderById<SearchFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(FILESYSTEM_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = FILESYSTEM_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), FILESYSTEM_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    FileSystemFolder? folder = ContentFolderWrapper.GetFolderById<FileSystemFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(SHORTCUT_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = SHORTCUT_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), SHORTCUT_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    ShortCutFolder? folder = ContentFolderWrapper.GetFolderById<ShortCutFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(OUTLOOK_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = OUTLOOK_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), OUTLOOK_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    OutlookFolder? folder = ContentFolderWrapper.GetFolderById<OutlookFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    EdgeBrowseHistoryFolder? folder = ContentFolderWrapper.GetFolderById<EdgeBrowseHistoryFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(RECENT_FILES_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = RECENT_FILES_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), RECENT_FILES_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    RecentFilesFolder? folder = ContentFolderWrapper.GetFolderById<RecentFilesFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    ClipboardHistoryFolder? folder = ContentFolderWrapper.GetFolderById<ClipboardHistoryFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
        // ScreenShotHistory Root Folder
        private static ScreenShotHistoryFolder? screenShotHistoryRootFolder;
        public static ScreenShotHistoryFolder ScreenShotHistoryRootFolder {
            get {
                if (screenShotHistoryRootFolder == null) {
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), SCREEN_SHOT_HISTORY_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    ScreenShotHistoryFolder? folder = ContentFolderWrapper.GetFolderById<ScreenShotHistoryFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), INTEGRATED_MONITOR_HISTORY_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    IntegratedMonitorHistoryFolder? folder = ContentFolderWrapper.GetFolderById<IntegratedMonitorHistoryFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
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