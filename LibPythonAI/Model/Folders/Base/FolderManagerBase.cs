using System.IO;
using LibPythonAI.Common;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Resources;

namespace LibPythonAI.Model.Folders.Base {
    public class FolderManagerBase {

        public static readonly string CHAT_ROOT_FOLDER_NAME = PythonAILibStringResources.Instance.ChatHistory;

        // 英語名
        public static readonly string CHAT_ROOT_FOLDER_NAME_EN = PythonAILibStringResources.Instance.ChatHistoryEnglish;


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
                            Id = folderRoot.Id,
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