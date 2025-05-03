using System.IO;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Folders.Outlook;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.Model.Folders.ShortCut;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;
using LibUIPythonAI.Resource;
using NetOffice.OutlookApi;
using PythonAILib.Common;

namespace AIChatExplorer.Model.Main {
    public class AIChatExplorerFolderManager : FolderManager {

        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME = CommonStringResources.Instance.Clipboard;
        public static readonly string SEARCH_ROOT_FOLDER_NAME = CommonStringResources.Instance.SearchFolder;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME = CommonStringResources.Instance.ImageChat;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME = CommonStringResources.Instance.FileSystem;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME = CommonStringResources.Instance.Shortcut;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME = CommonStringResources.Instance.Outlook;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME = CommonStringResources.Instance.EdgeBrowseHistory;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME = CommonStringResources.Instance.RecentFiles;

        // 英語名
        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ClipboardEnglish;
        public static readonly string SEARCH_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.SearchFolderEnglish;
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ImageChatEnglish;
        public static readonly string FILESYSTEM_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.FileSystemEnglish;
        public static readonly string SHORTCUT_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.ShortcutEnglish;
        public static readonly string OUTLOOK_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.OutlookEnglish;
        public static readonly string EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.EdgeBrowseHistoryEnglish;
        public static readonly string RECENT_FILES_ROOT_FOLDER_NAME_EN = CommonStringResources.Instance.RecentFilesEnglish;




        #region static methods

        // 言語変更時にルートフォルダ名を変更する
        public static void ChangeRootFolderNames(CommonStringResources toRes) {
            using PythonAILibDBContext db = new();
            // ClipboardRootFolder

            var clipboardRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == CLIPBOARD_ROOT_FOLDER_NAME_EN);

            if (clipboardRootFolder != null) {
                clipboardRootFolder.FolderName = toRes.Clipboard;
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
        }

        //--------------------------------------------------------------------------------
        private static ClipboardFolder? clipboardRootFolder;
        public static ClipboardFolder RootFolder {
            get {
                if (clipboardRootFolder == null) {
                    using PythonAILibDBContext db = new();
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(CLIPBOARD_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = CLIPBOARD_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CLIPBOARD_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    ClipboardFolder? folder = ContentFolderWrapper.GetFolderById<ClipboardFolder>(folderRoot.Id);
                    if (folder == null) {
                        folder = new() {
                            FolderName = CLIPBOARD_ROOT_FOLDER_NAME,
                            FolderTypeString = CLIPBOARD_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }
                    clipboardRootFolder = folder;

                }
                return clipboardRootFolder;
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
                            FolderName = SEARCH_ROOT_FOLDER_NAME,
                            FolderTypeString = SEARCH_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }
                    searchRootFolder = folder;
                }
                return searchRootFolder;
            }
        }
        private static ClipboardFolder? chatRootFolder;

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
                            FolderName = FILESYSTEM_ROOT_FOLDER_NAME,
                            FolderTypeString = FILESYSTEM_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }

                    fileSystemRootFolder = folder;
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
                            FolderName = SHORTCUT_ROOT_FOLDER_NAME,
                            FolderTypeString = SHORTCUT_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }

                    shortcutRootFolder = folder;
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
                            FolderName = OUTLOOK_ROOT_FOLDER_NAME,
                            FolderTypeString = OUTLOOK_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }

                    outlookRootFolder = folder;
                }
                return outlookRootFolder;
                #endregion
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
                            FolderName = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME,
                            FolderTypeString = EDGE_BROWSE_HISTORY_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }

                    edgeBrowseHistoryRootFolder = folder;
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
                            FolderName = RECENT_FILES_ROOT_FOLDER_NAME,
                            FolderTypeString = RECENT_FILES_ROOT_FOLDER_NAME_EN,
                        };
                        folder.Save();
                    }
                    recentFilesRootFolder = folder;
                }

                return recentFilesRootFolder;
            }
        }

    }
}