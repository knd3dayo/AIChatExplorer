using PythonAILib.Model.File;
using PythonAILib.PythonIF;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace ClipboardApp.Model {
    internal class DevelopmentFeatures {
    }

    public partial class ClipboardItem {


        public static void CreateAutoTitle(ClipboardItem item) {
            // TextとImageの場合
            if (item.ContentType == ContentTypes.ContentItemTypes.Text || item.ContentType == ContentTypes.ContentItemTypes.Image) {
                item.Description = $"{item.SourceApplicationTitle}";
            }
            // Fileの場合
            else if (item.ContentType == ContentTypes.ContentItemTypes.Files) {
                item.Description = $"{item.SourceApplicationTitle}";
                // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                if (item.Content.Length > 20) {
                    item.Description += $" {CommonStringResources.Instance.File}:" + item.Content[..20] + "..." + item.Content[^30..];
                } else {
                    item.Description += $" {CommonStringResources.Instance.File}:" + item.Content;
                }
            }
        }


        // 自動でタグを付与するコマンド
        public static void CreateAutoTags(ClipboardItem item) {
            // PythonでItem.ContentからEntityを抽出
            string spacyModel = Properties.Settings.Default.SpacyModel;
            HashSet<string> entities = PythonExecutor.PythonMiscFunctions.ExtractEntity(spacyModel, item.Content);
            foreach (var entity in entities) {

                // タグを追加
                item.Tags.Add(entity);
            }

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public ClipboardItem MaskDataCommandExecute() {

            if (this.ContentType != ContentTypes.ContentItemTypes.Text) {
                LogWrapper.Info(CommonStringResources.Instance.CannotMaskNonTextContent);
                return this;
            }
            string spacyModel = Properties.Settings.Default.SpacyModel;
            string result = PythonExecutor.PythonMiscFunctions.GetMaskedString(spacyModel, this.Content);
            this.Content = result;

            LogWrapper.Info(CommonStringResources.Instance.MaskedData);
            return this;
        }

        public static string CovertMaskedDataToOriginalData(MaskedData? maskedData, string maskedText) {
            if (maskedData == null) {
                return maskedText;
            }
            // マスキングデータをもとに戻す
            string result = maskedText;
            foreach (var entity in maskedData.Entities) {
                // ステータスバーにメッセージを表示
                LogWrapper.Info($"{CommonStringResources.Instance.RestoreMaskingData}: {entity.Before} -> {entity.After}\n");
                result = result.Replace(entity.After, entity.Before);
            }
            return result;
        }
    }
}
