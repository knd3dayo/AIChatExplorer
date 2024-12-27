using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using ClipboardApp.Factory;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using QAChat.Resource;
using WpfAppCommon.Utils;


namespace ClipboardApp.Model {
    public partial class ClipboardItem : ContentItem {
        // コンストラクタ
        public ClipboardItem(LiteDB.ObjectId folderObjectId) {


            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            CollectionId = folderObjectId;
        }

        // プロパティ
        public string FolderPath {
            get {
                // FolderObjectIdからClipboardFolderを取得
                ClipboardFolder? folder = (ClipboardFolder?)ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>().FindById(CollectionId);
                if (folder == null) {
                    return "";
                }
                return folder.FolderPath;
            }
        }


        // -------------------------------------------------------------------
        // インスタンスメソッド
        // -------------------------------------------------------------------
        public override object Copy() {
            ClipboardItem clipboardItem = new(this.CollectionId);
            CopyTo(clipboardItem);
            return clipboardItem;
        }



        //--------------------------------------------------------------------------------
        // staticメソッド
        //--------------------------------------------------------------------------------

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson<T>(T item) where T: ContentItem {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }


        // JSON文字列をClipboardItemに変換する
        public static T? FromJson<T>(string json) where T: ContentItem{
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            T? item = System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
            if (item == null) {
                LogWrapper.Error(CommonStringResources.Instance.FailedToParseJSONString);
                return null;
            }
            return item;

        }

        // 自動処理を適用する処理
        public ClipboardItem? ApplyAutoProcess() {

            ClipboardItem? result = this;
            // AutoProcessRulesを取得
            var AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(this.GetFolder<ClipboardFolder>());
            foreach (var rule in AutoProcessRules) {
                LogWrapper.Info($"{CommonStringResources.Instance.ApplyAutoProcessing} {rule.GetDescriptionString()}");
                result = rule.RunAction(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    LogWrapper.Info(CommonStringResources.Instance.ItemsDeletedByAutoProcessing);
                    return null;
                }
            }
            return result;
        }

    }
}
