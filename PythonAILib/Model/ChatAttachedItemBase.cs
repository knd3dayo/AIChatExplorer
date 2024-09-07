using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;

namespace PythonAILib.Model {
    public abstract class ChatAttachedItemBase {


        // LiteDBで保存するためのBase64文字列
        [BsonIgnore]
        public string? Base64String {
            get {
                // use cacheの場合はキャッシュを使用する
                if (UseCache) {
                    return CachedBase64String;
                }
                // キャッシュがない場合はファイルから取得する
                byte[] bytes = GetData();
                return System.Convert.ToBase64String(bytes);
            }
        }

        public string? CachedBase64String { get; set; }

        // LiteDBで保存されたキャッシュを使用するか否か？
        public bool UseCache { get; set; } = false;

        // ファイルパス
        public string FilePath { get; set; } = "";

        // ファイルの一時的なデータ
        [BsonIgnore]
        public byte[]? TempData { get; set; }

        // 抽出したテキスト
        [BsonIgnore]
        public string ExtractedText { get; set; } = "";

        // フォルダ名
        [BsonIgnore]
        public string FolderName {
            get {
                return System.IO.Path.GetDirectoryName(FilePath) ?? "";
            }
        }
        // ファイル名
        [BsonIgnore]
        public string FileName {
            get {
                return System.IO.Path.GetFileName(FilePath) ?? "";
            }
        }
        // フォルダ名 + \n + ファイル名
        [BsonIgnore]
        public string FolderAndFileName {
            get {
                return FolderName + "\n" + FileName;
            }
        }

        // 画像イメージ
        [BsonIgnore]
        public BitmapImage? BitmapImage {
            get {
                byte[] imageBytes = GetData();
                if (!ContentTypes.IsImageData(imageBytes)) {
                    return null;
                }
                return PythonAILib.Model.ContentTypes.GetBitmapImage(imageBytes);
            }
        }
        [BsonIgnore]
        public Image? Image {
            get {
                byte[] imageBytes = GetData();
                if (!ContentTypes.IsImageData(imageBytes)) {
                    return null;
                }
                return PythonAILib.Model.ContentTypes.GetImageFromBase64(Base64String ?? "");
            }
        }

        public bool IsImage() {
            byte[] imageBytes = GetData();
            return ContentTypes.IsImageData(imageBytes);
        }

        // キャッシュを更新する
        public void UpdateCache() {
            if (FilePath == null || File.Exists(FilePath) == false) {
                return;
            }
            byte[] bytes = System.IO.File.ReadAllBytes(FilePath);
            CachedBase64String = System.Convert.ToBase64String(bytes);
        }
        // キャッシュからファイルデータを取得する
        public byte[] GetCachedData() {
            if (CachedBase64String == null) {
                return [];
            }
            return System.Convert.FromBase64String(CachedBase64String);
        }
        // ファイルまたはキャッシュからデータを取得する
        public byte[] GetData() {
            if (TempData != null) {
                return TempData;
            }
            if (UseCache || FilePath == null || File.Exists(FilePath) == false) {
                return GetCachedData();
            } else {
                return System.IO.File.ReadAllBytes(FilePath);
            }
        }

        // 削除
        public abstract void Delete();
        // 保存
        public abstract void Save();
        // Embedding更新
        public abstract void UpdateEmbedding();

        // テキストを抽出する
        public abstract void ExtractText();

    }
}
