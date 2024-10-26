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

        // LiteDBに保存するためのBase64文字列. 元ファイルまたは画像データをBase64エンコードした文字列
        private string _cachedBase64String = "";
        public string CachedBase64String {
            get {
                return _cachedBase64String;
            }
            set {
                if (value == null) {
                    _cachedBase64String = string.Empty;
                } else {
                    _cachedBase64String = value;
                }
            }
        }
        // ファイルパス
        public string FilePath { get; set; } = "";
        // ファイルの最終更新日時
        public long LastModified { get; set; } = 0;

        [BsonIgnore]
        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(FileName)) {
                    return "No Name";
                }
                return FileName;
            }
        }

        [BsonIgnore]
        public string Base64String {
            get {

                // FilePathがない場合はキャッシュを返す
                if (FilePath == null || System.IO.File.Exists(FilePath) == false) {
                    return CachedBase64String;
                }
                // FilePathがある場合はLastModifiedをチェックしてキャッシュを更新する
                if (LastModified < new System.IO.FileInfo(FilePath).LastWriteTime.Ticks) {
                    UpdateCache();
                }
                return CachedBase64String;
            }
        }

        // 抽出したテキスト
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
                return FolderName + Path.PathSeparator + FileName;
            }
        }

        // 画像イメージ
        [BsonIgnore]
        public BitmapImage? BitmapImage {
            get {
                if (!IsImage()) {
                    return null;
                }
                byte[] imageBytes = Convert.FromBase64String(Base64String);
                return ContentTypes.GetBitmapImage(imageBytes);
            }
        }
        [BsonIgnore]
        public System.Drawing.Image? Image {
            get {
                if (!IsImage()) {
                    return null;
                }
                return ContentTypes.GetImageFromBase64(Base64String);
            }
        }

        public bool IsImage() {
            if (Base64String == null) {
                return false;
            }
            return ContentTypes.IsImageData(Convert.FromBase64String(Base64String));
        }

        // キャッシュを更新する
        public void UpdateCache() {
            if (FilePath == null || System.IO.File.Exists(FilePath) == false) {
                return;
            }
            LastModified = new System.IO.FileInfo(FilePath).LastWriteTime.Ticks;

            byte[] bytes = System.IO.File.ReadAllBytes(FilePath);
            CachedBase64String = Convert.ToBase64String(bytes);
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
            // キャッシュを更新
            UpdateCache();

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            string base64 = Base64String;

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
