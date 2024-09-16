using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;
using PythonAILib.Model.Abstract;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Model
{
    public class ClipboardItemImage_ : ImageItemBase {

        public ObjectId Id { get; set; } = ObjectId.Empty;

        [BsonIgnore]
        public ClipboardItem? ClipboardItem { get; set; }

        public static ClipboardItemImage_ Create(ClipboardItem clipboardItem, Image image) {
            ClipboardItemImage_ itemImage = new() {
                ClipboardItem = clipboardItem,
                Image = image
            };
            return itemImage;
        }




        // 削除
        public override void Delete() {
            string idString = Id.ToString();

            Task.Run(() => {
                if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                    // SyncFolderName/フォルダ名/ファイル名を削除する
                    string syncFolderName = ClipboardAppConfig.SyncFolderName;
                    if (ClipboardItem == null) {
                        throw new Exception("FilePath is null");
                    }
                    string syncFolder = Path.Combine(syncFolderName, ClipboardItem.FolderPath);
                    string syncFilePath = Path.Combine(syncFolder, idString);
                    if (File.Exists(syncFilePath)) {
                        File.Delete(syncFilePath);
                    }
                    // 自動コミットが有効の場合はGitにコミット
                    if (ClipboardAppConfig.AutoCommit) {
                        ClipboardItem.GitCommit(syncFilePath);
                    }
                }
            });
            // Embeddingを削除
            if (ClipboardItem != null) {
                LogWrapper.Info(CommonStringResources.Instance.DeleteTextEmbeddingFromImage);
                VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(ClipboardItem.GetFolder());
                // ImageInfoを作成
                ImageInfo imageInfo = new(VectorDBUpdateMode.delete, idString);
                folderVectorDBItem.DeleteIndex(imageInfo);
                LogWrapper.Info(CommonStringResources.Instance.DeletedTextEmbeddingFromImage);
            }

                // ★TODO Test ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItemImage(this);

        }
        // 保存
        public override void Save() {
            // ★TODO Test
            // ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItemImage(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                if (ClipboardItem == null) {
                    throw new Exception("FilePath is null");
                }
                // SyncFolderName/フォルダ名/ファイル名にファイルを保存する
                string syncFolderName = ClipboardAppConfig.SyncFolderName;
                string syncFolder = Path.Combine(syncFolderName, ClipboardItem.FolderPath);
                string syncFilePath = Path.Combine(syncFolder, Id.ToString());
                if (!Directory.Exists(syncFolder)) {
                    Directory.CreateDirectory(syncFolder);
                }
                if (Image == null) {
                    throw new Exception("image is null");
                }
                Image.Save(syncFilePath, System.Drawing.Imaging.ImageFormat.Png);

                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.AutoCommit) {
                    ClipboardItem.GitCommit(syncFilePath);
                }
            }
        }

        // Embeddingを生成
        // Embeddingを更新する
        public void UpdateEmbedding() {
            if (ClipboardItem == null) {
                throw new Exception("ClipboardItem is null");
            }
            LogWrapper.Info(CommonStringResources.Instance.SaveTextEmbeddingFromImage);

            ImageInfo imageInfo = new(VectorDBUpdateMode.update, Id.ToString(), ImageBase64);

            // VectorDBItemを取得
            VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(ClipboardItem.GetFolder());
            // Embeddingを保存
            folderVectorDBItem.UpdateIndex(imageInfo);
            LogWrapper.Info(CommonStringResources.Instance.SavedTextEmbeddingFromImage);
        }

    }
}
