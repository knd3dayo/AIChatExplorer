using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class ClipboardItemImage {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        public ClipboardItem? ClipboardItem { get; set; }

        public static ClipboardItemImage Create(ClipboardItem clipboardItem, Image image) {
            ClipboardItemImage itemImage = new() {
                ClipboardItem = clipboardItem,
                Image = image
            };
            return itemImage;
        }

        // 画像イメージのBase64文字列
        public string ImageBase64 { get; set; } = String.Empty;

        // 画像イメージ
        [BsonIgnore]
        public Image? Image {
            get {
                if (string.IsNullOrEmpty(ImageBase64)) {
                    return null;
                }
                byte[] imageBytes = Convert.FromBase64String(ImageBase64);
                using MemoryStream ms = new(imageBytes);
                return Image.FromStream(ms);
            }
            set {
                using MemoryStream ms = new();
                value?.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ImageBase64 = Convert.ToBase64String(ms.ToArray());
            }
        }
        [BsonIgnore]
        public BitmapImage? BitmapImage {
            get {
                if (string.IsNullOrEmpty(ImageBase64)) {
                    return null;
                }
                byte[] binaryData = Convert.FromBase64String(ImageBase64);
                MemoryStream ms = new(binaryData, 0, binaryData.Length);
                BitmapImage bi = new();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }
        // 画像データのサムネイル
        [BsonIgnore]
        public Image? ThumbnailImage {
            get {
                if (Image == null) {
                    return null;
                }
                return Image.GetThumbnailImage(100, 100, () => false, IntPtr.Zero);
            }

        }
        // 画像データのサムネイルのBitmapImage
        [BsonIgnore]
        public BitmapImage? ThumbnailBitmapImage {
            get {
                Image? image = ThumbnailImage;
                if (image == null) {
                    return null;
                }
                MemoryStream ms = new();
                Image?.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                BitmapImage bi = new();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
        }

        // 削除
        public void Delete() {
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

            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItemImage(this);

        }
        // 保存
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItemImage(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                if (ClipboardItem == null) {
                    throw new Exception("FilePath is null");
                }
                // SyncFolderName/フォルダ名/ファイル名にファイルを保存する
                string syncFolderName = ClipboardAppConfig.SyncFolderName;
                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardItem.FolderPath);
                string syncFilePath = System.IO.Path.Combine(syncFolder, Id.ToString());
                if (!System.IO.Directory.Exists(syncFolder)) {
                    System.IO.Directory.CreateDirectory(syncFolder);
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

            ImageInfo imageInfo = new(VectorDBUpdateMode.update, this.Id.ToString(), ImageBase64);

            // VectorDBItemを取得
            VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(ClipboardItem.GetFolder());
            // Embeddingを保存
            folderVectorDBItem.UpdateIndex(imageInfo);
            LogWrapper.Info(CommonStringResources.Instance.SavedTextEmbeddingFromImage);
        }

        // 取得
        public static ClipboardItemImage? GetItems(LiteDB.ObjectId objectId) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(objectId);
        }
    }
}
