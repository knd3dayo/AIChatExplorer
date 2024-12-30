using ClipboardApp.Model.Folder;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using QAChat.Resource;
using WpfAppCommon.Utils;


namespace ClipboardApp.Model.Item {
    public partial class ClipboardItem : ContentItem {
        // コンストラクタ
        public ClipboardItem(LiteDB.ObjectId folderObjectId) {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            CollectionId = folderObjectId;
        }

        public override object Copy() {
            ClipboardItem clipboardItem = new(this.CollectionId);
            CopyTo(clipboardItem);
            return clipboardItem;
        }

        // 自動処理を適用する処理
        public ContentItem? ApplyAutoProcess() {

            ContentItem? result = this;
            // AutoProcessRulesを取得
            var AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(this.GetFolder<ClipboardFolder>());
            foreach (var rule in AutoProcessRules) {
                LogWrapper.Info($"{CommonStringResources.Instance.ApplyAutoProcessing} {rule.GetDescriptionString()}");
                rule.RunAction(result);
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
