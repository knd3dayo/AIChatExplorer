using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Resource;
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
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(GetItems<ApplicationItem>(isSync: false), jsonSerializerOptions);

            System.IO.File.WriteAllText(fileName, jsonString);

        }

        // ステータス用のテキストを作成
        public override string GetStatusText() {
            string message = $"{CommonStringResources.Instance.Folder}[{FolderName}]";
            // AutoProcessRuleが設定されている場合
            var rules = AutoProcessRuleController.GetAutoProcessRules(this);
            if (rules.Count > 0) {
                message += $" {CommonStringResources.Instance.AutoProcessingIsSet}[";
                foreach (AutoProcessRule item in rules) {
                    message += item.RuleName + " ";
                }
                message += "]";
            }
            return message;

        }

    }
}

