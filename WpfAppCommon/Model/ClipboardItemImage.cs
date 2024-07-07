using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using LibGit2Sharp;
using LiteDB;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class ClipboardItemImage {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        public ClipboardItem? ClipboardItem { get; set; }

        public ClipboardItemImage() {

        }
        public static ClipboardItemImage Create(ClipboardItem clipboardItem, Image image) {
            ClipboardItemImage itemImage = new();
            itemImage.ClipboardItem = clipboardItem;
            itemImage.Image = image;
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
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItemImage(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                // SyncFolderName/フォルダ名/ファイル名を削除する
                string syncFolderName = ClipboardAppConfig.SyncFolderName;
                if (ClipboardItem == null) {
                    throw new Exception("FilePath is null");
                }
                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardItem.FolderPath);
                string syncFilePath = System.IO.Path.Combine(syncFolder, Id.ToString());
                if (System.IO.File.Exists(syncFilePath)) {
                    System.IO.File.Delete(syncFilePath);
                }
                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.AutoCommit) {
                    try {
                        using (var repo = new Repository(ClipboardAppConfig.SyncFolderName)) {
                            Commands.Stage(repo, syncFilePath);
                            Signature author = new("ClipboardApp", "ClipboardApp", DateTimeOffset.Now);
                            Signature committer = author;
                            repo.Commit("Auto commit", author, committer);
                            LogWrapper.Info($"Gitにコミットしました:{syncFilePath} {ClipboardAppConfig.SyncFolderName}");
                        }
                    } catch (RepositoryNotFoundException e) {
                        LogWrapper.Info($"リポジトリが見つかりませんでした:{ClipboardAppConfig.SyncFolderName} {e.Message}");
                    } catch (EmptyCommitException e) {
                        LogWrapper.Info($"コミットが空です:{syncFilePath} {e.Message}");
                    }
                }
            }
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
                    try {
                        using (var repo = new Repository(ClipboardAppConfig.SyncFolderName)) {
                            Commands.Stage(repo, syncFilePath);
                            Signature author = new("ClipboardApp", "ClipboardApp", DateTimeOffset.Now);
                            Signature committer = author;
                            repo.Commit("Auto commit", author, committer);
                            LogWrapper.Info($"Gitにコミットしました:{syncFilePath} {ClipboardAppConfig.SyncFolderName}");
                        }
                    } catch (RepositoryNotFoundException e) {
                        LogWrapper.Info($"リポジトリが見つかりませんでした:{ClipboardAppConfig.SyncFolderName} {e.Message}");
                    } catch (EmptyCommitException e) {
                        LogWrapper.Info($"コミットが空です:{syncFilePath} {e.Message}");
                    }
                }
            }
        }

        // 取得
        public static ClipboardItemImage? GetItems(LiteDB.ObjectId objectId) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(objectId);
        }
    }
}
