using PythonAILib.PythonIF;
using WpfAppCommon.Utils;
using System.Drawing;

namespace WpfAppCommon.Model {
    internal class DevelopmentFeatures {
    }

    public partial class ClipboardItem {


        public static void CreateAutoTitle(ClipboardItem item) {
            // TextとImageの場合
            if (item.ContentType == ClipboardContentTypes.Text || item.ContentType == ClipboardContentTypes.Image) {
                item.Description = $"{item.SourceApplicationTitle}";
            }
            // Fileの場合
            else if (item.ContentType == ClipboardContentTypes.Files) {
                item.Description = $"{item.SourceApplicationTitle}";
                // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                if (item.Content.Length > 20) {
                    item.Description += " ファイル：" + item.Content[..20] + "..." + item.Content[^30..];
                } else {
                    item.Description += " ファイル：" + item.Content;
                }
            }
        }


        // 自動でタグを付与するコマンド
        public static void CreateAutoTags(ClipboardItem item) {
            // PythonでItem.ContentからEntityを抽出
            string spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
            HashSet<string> entities = PythonExecutor.PythonMiscFunctions.ExtractEntity(spacyModel, item.Content);
            foreach (var entity in entities) {

                // タグを追加
                item.Tags.Add(entity);
            }

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public ClipboardItem MaskDataCommandExecute() {

            if (this.ContentType != ClipboardContentTypes.Text) {
                LogWrapper.Info("テキスト以外のコンテンツはマスキングできません");
                return this;
            }
            string spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
            string result = PythonExecutor.PythonMiscFunctions.GetMaskedString(spacyModel, this.Content);
            this.Content = result;

            LogWrapper.Info("データをマスキングしました");
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
                LogWrapper.Info($"マスキングデータをもとに戻します: {entity.Before} -> {entity.After}\n");
                result = result.Replace(entity.After, entity.Before);
            }
            return result;
        }

        // 画像からイメージを抽出するコマンド
        public static ClipboardItem ExtractTextFromImageCommandExecute(ClipboardItem clipboardItem) {
            if (clipboardItem.ContentType != ClipboardContentTypes.Image) {
                throw new ThisApplicationException("画像以外のコンテンツはテキストを抽出できません");
            }
            foreach (var imageObjectId in clipboardItem.ImageObjectIds) {
                ClipboardItemImage? imageItem = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(imageObjectId);
                if (imageItem == null) {
                    throw new ThisApplicationException("画像が取得できません");
                }
                Image? image = imageItem.Image;
                if (image == null) {
                    throw new ThisApplicationException("画像が取得できません");
                }
                string text = PythonExecutor.PythonMiscFunctions.ExtractTextFromImage(image, ClipboardAppConfig.TesseractExePath);
                clipboardItem.Content += text + "\n";
            }

            return clipboardItem;
        }



    }
}
