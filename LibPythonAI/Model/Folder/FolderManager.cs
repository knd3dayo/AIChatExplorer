using System.IO;
using LibPythonAI.Common;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Resources;

namespace LibPythonAI.Model.Folder {
    public class FolderManager {

        public static readonly string CHAT_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.ChatHistory;

        // 英語名
        public static readonly string CHAT_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ChatHistoryEnglish;

        
        // 言語変更時にルートフォルダ名を変更する
        public static void ChangeRootFolderNames(PythonAILibStringResources toRes) {
            using PythonAILibDBContext db = new();
            // ChatRootFolder
            var chatRootFolder = db.ContentFolders.FirstOrDefault(x => x.ParentId == null && x.FolderTypeString == CHAT_ROOT_FOLDER_NAME_EN);
            if (chatRootFolder != null) {
                chatRootFolder.FolderName = toRes.ChatHistory;
            }
        }

        //--------------------------------------------------------------------------------
        private static ContentFolderWrapper? chatRootFolder;

        public static ContentFolderWrapper ChatRootFolder {
            get {
                if (chatRootFolder == null) {
                    ContentFolderRoot? folderRoot = ContentFolderRoot.GetFolderRootByFolderType(CHAT_ROOT_FOLDER_NAME_EN);
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = CHAT_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CHAT_ROOT_FOLDER_NAME_EN)
                        };
                        folderRoot.Save();
                    }
                    ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById< ContentFolderWrapper>(folderRoot.Id);
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
    }
}