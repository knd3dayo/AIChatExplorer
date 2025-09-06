using System.Text.Json;
using LibMain.Data;
using LibMain.Model.AutoProcess;
using LibMain.Model.Content;
using LibMain.Model.Folders;
using LibMain.Resources;
using LibMain.Utils.Common;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace AIChatExplorer.Model.Folders.Application {
    public partial class ApplicationFolder : ContentFolderWrapper {


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ApplicationFolder() : base() {
            FolderTypeString = FolderManager.APPLICATION_ROOT_FOLDER_NAME_EN;
        }

        protected ApplicationFolder(ApplicationFolder? parent, string folderName) : base(parent, folderName) {
            FolderTypeString = FolderManager.APPLICATION_ROOT_FOLDER_NAME_EN;
        }


        public override ApplicationFolder CreateChild(string folderName) {
            ContentFolderEntity childFolder = new() {
                ParentId = Id,
                FolderName = folderName,
                FolderTypeString = FolderTypeString,

            };
            ApplicationFolder child = new() { Entity = childFolder };
            return child;
        }

        /// <summary>
        /// Set application information from ClipboardChangedEventArgs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sender"></param>
        public static void SetApplicationInfo(ApplicationItem item, ClipboardChangedEventArgs sender) {
            item.SourceApplicationName = sender.SourceApplication.Name;
            item.SourceApplicationTitle = sender.SourceApplication.Title;
            item.SourceApplicationID = sender.SourceApplication.ID;
            item.SourceApplicationPath = sender.SourceApplication.Path;
        }


        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string fileName) {
            string jsonString = JsonSerializer.Serialize(GetItemsAsync<ApplicationItem>(isSync: false), JsonUtil.JsonSerializerOptions);
            System.IO.File.WriteAllText(fileName, jsonString);

        }

        // ステータス用のテキストを作成
        public override async Task<string> GetStatusText() {
            string message = $"{PythonAILibStringResources.Instance.Folder}[{FolderName}]";
            // AutoProcessRuleが設定されている場合
            var rules = await AutoProcessRuleController.GetAutoProcessRules(this);
            if (rules.Count > 0) {
                message += $" {PythonAILibStringResources.Instance.AutoProcessingIsSet}[";
                foreach (AutoProcessRule item in rules) {
                    message += item.RuleName + " ";
                }
                message += "]";
            }
            return message;

        }

    }
}

