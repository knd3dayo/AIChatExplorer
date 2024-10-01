using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils;
using QAChat;

namespace PythonAILib.Model.Content {
    public class ContentAttachedItem {

        public ObjectId Id { get; set; } = ObjectId.Empty;

        // LiteDBで保存するためのBase64文字列
        [BsonIgnore]
        public string Base64String {
            get {
                // use cacheの場合はキャッシュを使用する
                if (UseCache) {
                    return CachedBase64String ?? "";
                }
                // キャッシュがない場合はファイルから取得する
                byte[] bytes = GetData();
                return Convert.ToBase64String(bytes);
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
                return Path.GetDirectoryName(FilePath) ?? "";
            }
        }
        // ファイル名
        [BsonIgnore]
        public string FileName {
            get {
                return Path.GetFileName(FilePath) ?? "";
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
                if (imageBytes == null || !ContentTypes.IsImageData(imageBytes)) {
                    return null;
                }
                return ContentTypes.GetBitmapImage(imageBytes);
            }
        }
        [BsonIgnore]
        public System.Drawing.Image? Image {
            get {
                byte[] imageBytes = GetData();
                if (imageBytes == null || !ContentTypes.IsImageData(imageBytes)) {
                    return null;
                }
                return ContentTypes.GetImageFromBase64(Base64String ?? "");
            }
        }

        public bool IsImage() {
            byte[] imageBytes = GetData();
            return ContentTypes.IsImageData(imageBytes);
        }

        // キャッシュを更新する
        public void UpdateCache() {
            if (FilePath == null || System.IO.File.Exists(FilePath) == false) {
                return;
            }
            byte[] bytes = System.IO.File.ReadAllBytes(FilePath);
            CachedBase64String = Convert.ToBase64String(bytes);
        }
        // キャッシュからファイルデータを取得する
        public byte[] GetCachedData() {
            if (CachedBase64String == null) {
                return [];
            }
            return Convert.FromBase64String(CachedBase64String);
        }
        // ファイルまたはキャッシュからデータを取得する
        public byte[] GetData() {
            if (TempData != null) {
                return TempData;
            }
            if (UseCache || FilePath == null || System.IO.File.Exists(FilePath) == false) {
                return GetCachedData();
            } else {
                return System.IO.File.ReadAllBytes(FilePath);
            }
        }

        // 削除
        public virtual void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.DeleteAttachedItem(this);
        }
        // 保存
        public virtual void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.UpsertAttachedItem(this);
        }

        // テキストを抽出する
        public void ExtractText() {

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            byte[] data = GetData();
            string base64 = Convert.ToBase64String(data);
            try {

                if (ContentTypes.IsImageData(base64)) {
                    string result = ChatUtil.ExtractTextFromImage(openAIProperties, [base64]);
                    if (string.IsNullOrEmpty(result) == false) {
                        ExtractedText = result;
                    }
                } else {
                    string text = PythonExecutor.PythonAIFunctions.ExtractBase64ToText(base64);
                    ExtractedText = text;
                }

            } catch (UnsupportedFileTypeException) {
                LogWrapper.Info(PythonAILibStringResources.Instance.UnsupportedFileType);
            }
        }

        // 取得
        public static ContentAttachedItem? GetItem(ObjectId objectId) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            return libManager.DataFactory.GetAttachedItem(objectId);
        }
    }
}
