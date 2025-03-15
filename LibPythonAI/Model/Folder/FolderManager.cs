using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using PythonAILib.Common;
using PythonAILib.Resources;

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
                    using PythonAILibDBContext db = new();
                    ContentFolderRootEntity? folderRoot = db.ContentFolderRoots.Where(x => x.FolderTypeString == CHAT_ROOT_FOLDER_NAME_EN).FirstOrDefault();
                    if (folderRoot == null) {
                        folderRoot = new() {
                            FolderTypeString = CHAT_ROOT_FOLDER_NAME_EN,
                            ContentOutputFolderPrefix = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetContentOutputPath(), CHAT_ROOT_FOLDER_NAME_EN)
                        };
                        db.ContentFolderRoots.Add(folderRoot);
                        db.SaveChanges();
                    }
                    ContentFolderEntity? folder = db.ContentFolders.Where(x => x.Id == folderRoot.Id).FirstOrDefault();
                    if (folder == null) {
                        folder = new() {
                            Id = folderRoot.Id,
                            FolderName = CHAT_ROOT_FOLDER_NAME,
                            FolderTypeString = CHAT_ROOT_FOLDER_NAME_EN,
                        };
                        db.ContentFolders.Add(folder);
                        db.SaveChanges();
                    }
                    chatRootFolder = new ContentFolderWrapper(folder);
                }
                return chatRootFolder;
            }
        }
    }
}