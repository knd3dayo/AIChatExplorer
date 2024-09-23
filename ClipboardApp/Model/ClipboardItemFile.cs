using ClipboardApp.Factory;
using LiteDB;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Model {
    public class ClipboardItemFile : ContentAttachedItemBase {

        public static ClipboardItemFile Create(ClipboardItem clipboardItem, string filePath) {
            ClipboardItemFile itemFile = new() {
                ClipboardItem = clipboardItem,
                FilePath = filePath
            };
            return itemFile;
        }
        public static ClipboardItemFile Create(ClipboardItem clipboardItem, System.Drawing.Image image) {
            ClipboardItemFile itemFile = new() {
                ClipboardItem = clipboardItem,
                CachedBase64String = ContentTypes.GetBase64StringFromImage(image)
            };
            return itemFile;
        }
        public static ClipboardItemFile CreateFromBase64(ClipboardItem clipboardItem, string base64string) {
            ClipboardItemFile itemFile = new() {
                ClipboardItem = clipboardItem,
                CachedBase64String = base64string
            };
            return itemFile;
        }



        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        [BsonIgnore]
        public ClipboardItem? ClipboardItem { get; set; }

        // 削除
        public override void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItemFile(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.Instance.SyncClipboardItemAndOSFolder) {
                // SyncFolderName/フォルダ名/ファイル名を削除する
                string syncFolderName = ClipboardAppConfig.Instance.SyncFolderName;

                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardItem?.FolderPath ?? "");
                string syncFilePath = System.IO.Path.Combine(syncFolder, FileName);
                if (System.IO.File.Exists(syncFilePath)) {
                    System.IO.File.Delete(syncFilePath);
                }
                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.Instance.AutoCommit) {
                    ClipboardItem?.GitCommit(syncFilePath);
                }

            }
        }


        // 保存
        public override void Save() {

            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItemFile(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.Instance.SyncClipboardItemAndOSFolder) {
                if (FilePath == null) {
                    throw new Exception("FilePath is null");
                }
                // SyncFolderName/フォルダ名/ファイル名にファイルを保存する
                string syncFolderName = ClipboardAppConfig.Instance.SyncFolderName;
                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardItem?.FolderPath ?? "");
                string syncFilePath = System.IO.Path.Combine(syncFolder, FileName);
                if (!System.IO.Directory.Exists(syncFolder)) {
                    System.IO.Directory.CreateDirectory(syncFolder);
                }
                if (System.IO.File.Exists(FilePath)) {
                    System.IO.File.Copy(FilePath, syncFilePath, true);
                }
                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.Instance.AutoCommit) {
                    ClipboardItem?.GitCommit(syncFilePath);
                }
            }
            // 一時データを削除
            TempData = null;
        }

        // Embeddingを生成
        // Embeddingを更新する
        public override void UpdateEmbedding() {
            if (ClipboardItem == null) {
                throw new Exception("ClipboardItem is null");
            }
            string base64;
            if (UseCache) {
                base64 = Base64String ?? "";
            } else {
                byte[] bytes = System.IO.File.ReadAllBytes(FilePath);
                base64 = System.Convert.ToBase64String(bytes);
            }
            if (ContentTypes.IsImageData(base64)) {
                // 画像からテキスト抽出
                LogWrapper.Info(CommonStringResources.Instance.SaveTextEmbeddingFromImage);
                ImageInfo imageInfo = new(VectorDBUpdateMode.update, Id.ToString(), base64);
                // VectorDBItemを取得
                VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(ClipboardItem.GetFolder());
                // Embeddingを保存
                folderVectorDBItem.UpdateIndex(imageInfo);
                LogWrapper.Info(CommonStringResources.Instance.SavedTextEmbeddingFromImage);
            } else {
                LogWrapper.Info(CommonStringResources.Instance.SaveTextEmbeddingFromImage);
                // 画像以外のデータからテキスト抽出
                string content = PythonExecutor.PythonAIFunctions.ExtractBase64ToText(base64);
                ContentInfo contentInfo = new(VectorDBUpdateMode.update, Id.ToString(), content);
                // VectorDBItemを取得
                VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(ClipboardItem.GetFolder());
                // Embeddingを保存
                folderVectorDBItem.UpdateIndex(contentInfo);
                LogWrapper.Info(CommonStringResources.Instance.SavedTextEmbeddingFromImage);

            }
        }

        public override void ExtractText() {
            byte[] data = GetData();
            string base64 = System.Convert.ToBase64String(data);
            try {
                if (ContentTypes.IsImageData(base64)) {
                    string result = ChatUtil.ExtractTextFromImage(ClipboardAppConfig.Instance.CreateOpenAIProperties(), [base64]);
                    if (string.IsNullOrEmpty(result) == false) {
                        ExtractedText = result;
                    }
                } else {
                    string text = PythonExecutor.PythonAIFunctions.ExtractBase64ToText(base64);
                    ExtractedText = text;
                }

            } catch (UnsupportedFileTypeException) {
                LogWrapper.Info(CommonStringResources.Instance.UnsupportedFileType);
            }
        }

        // 取得
        public static ClipboardItemFile? GetItem(LiteDB.ObjectId objectId) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(objectId);
        }
    }

}
