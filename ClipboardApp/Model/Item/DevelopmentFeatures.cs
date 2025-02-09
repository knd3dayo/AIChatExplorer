using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.PythonIF;
using LibUIPythonAI.Resource;
using WpfAppCommon.Utils;
using LibPythonAI.PythonIF.Request;

namespace ClipboardApp.Model.Item {
    internal class DevelopmentFeatures
    {
    }

    public partial class ClipboardItem
    {





        // 自動でタグを付与するコマンド
        public static void CreateAutoTags(ContentItem item)
        {
            // PythonでItem.ContentからEntityを抽出
            string spacyModel = Properties.Settings.Default.SpacyModel;
            HashSet<string> entities = PythonExecutor.PythonMiscFunctions.ExtractEntity(spacyModel, item.Content);
            foreach (var entity in entities)
            {

                // タグを追加
                item.Tags.Add(entity);
            }

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public ClipboardItem MaskDataCommandExecute()
        {

            if (ContentType != ContentTypes.ContentItemTypes.Text)
            {
                LogWrapper.Info(CommonStringResources.Instance.CannotMaskNonTextContent);
                return this;
            }
            string spacyModel = Properties.Settings.Default.SpacyModel;
            string result = PythonExecutor.PythonMiscFunctions.GetMaskedString(spacyModel, Content);
            Content = result;

            LogWrapper.Info(CommonStringResources.Instance.MaskedData);
            return this;
        }

        public static string CovertMaskedDataToOriginalData(MaskedData? maskedData, string maskedText)
        {
            if (maskedData == null)
            {
                return maskedText;
            }
            // マスキングデータをもとに戻す
            string result = maskedText;
            foreach (var entity in maskedData.Entities)
            {
                // ステータスバーにメッセージを表示
                LogWrapper.Info($"{CommonStringResources.Instance.RestoreMaskingData}: {entity.Before} -> {entity.After}\n");
                result = result.Replace(entity.After, entity.Before);
            }
            return result;
        }
    }
}
