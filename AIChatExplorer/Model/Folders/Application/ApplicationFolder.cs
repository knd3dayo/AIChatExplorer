using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Resource;
using PythonAILib.Model.AutoProcess;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace AIChatExplorer.Model.Folders.Clipboard {
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

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public virtual void ProcessApplicationItem(ClipboardChangedEventArgs e, Action<ContentItemWrapper> _afterClipboardChanged) {

            // Get the cut/copied text.
            List<ApplicationItem> items = CreateApplicationItem(this, e);

            foreach (var item in items) {
                // Process clipboard applicationItem
                ClipboardController.ProcessApplicationItem(item, _afterClipboardChanged);
            }
        }
        #endregion


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

        /// Create ContentItem
        public static List<ApplicationItem> CreateApplicationItem(
            ApplicationFolder clipboardFolder, ClipboardChangedEventArgs e) {

            List<ApplicationItem> result = [];
            
            ContentItemTypes.ContentItemTypeEnum contentItemTypes;
            string sourceType;
            if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Text) {
                contentItemTypes = LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Text;
                sourceType = ContentSourceType.Application;
            } else if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Files) {
                contentItemTypes = LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Files;
                sourceType = ContentSourceType.File;
            } else if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Image) {
                contentItemTypes = LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Image;
                sourceType = ContentSourceType.Application;
            } else if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Other) {
                return result;
            } else {
                return result;
            }

            // If ContentType is Text, set text data
            if (contentItemTypes == LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Text) {
                ApplicationItem item = new(clipboardFolder.Entity) {
                    ContentType = contentItemTypes,
                    SourceType = sourceType
                };
                SetApplicationInfo(item, e);
                item.Content = (string)e.Content;
                result.Add(item);
                return result;
            }

            // If ContentType is BitmapImage, set image data
            if (contentItemTypes == LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Image) {
                ApplicationItem item = new(clipboardFolder.Entity) {
                    ContentType = contentItemTypes,
                    SourceType = sourceType

                };
                SetApplicationInfo(item, e);
                System.Drawing.Image image = (System.Drawing.Image)e.Content;
                // byte
                item.Base64Image = LibPythonAI.Model.Content.ContentItemTypes.GetBase64StringFromImage(image);
                result.Add(item);
                return result;
            }

            // If ContentType is Files, set file data
            if (contentItemTypes == LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Files) {
                string[] files = (string[])e.Content;

                // Get the cut/copied file/files.
                for (int i = 0; i < files.Length; i++) {
                    ApplicationItem item = new(clipboardFolder.Entity) {
                        ContentType = contentItemTypes,
                        SourceType = sourceType
                    };
                    SetApplicationInfo(item, e);
                    item.SourcePath = files[i];
                    item.LastModified = new System.IO.FileInfo(item.SourcePath).LastWriteTime.Ticks;
                    result.Add(item);
                }
                return result;
            }
            return result;
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

