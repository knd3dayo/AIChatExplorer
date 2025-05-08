using LibPythonAI.Data;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Resources;

namespace LibPythonAI.Model.Content {
    public class ContentFolderWrapper {

        public ContentFolderWrapper() { }

        public ContentFolderWrapper(ContentFolderWrapper? parent, string folderName) {
            Entity.ParentId = parent?.Id;
            Entity.FolderName = folderName;
        }

        public ContentFolderEntity Entity { get; protected set; } = new ContentFolderEntity();

        public string Id {
            get {
                return Entity.Id;
            }
        }


        // Parent
        public ContentFolderWrapper? Parent {
            get {
                using PythonAILibDBContext db = new();
                var parentFolder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.ParentId);
                if (parentFolder == null) {
                    return null;
                }
                return new ContentFolderWrapper() { Entity = parentFolder };
            }
            set {
                Entity.ParentId = value?.Entity.Id;
            }
        }

        // フォルダの種類の文字列
        public string FolderTypeString {
            get {
                return Entity.FolderTypeString;
            }
            set {
                Entity.FolderTypeString = value;
            }
        }
        public bool IsAutoProcessEnabled { get; set; } = true;

        // アプリケーション内でのフォルダのパス
        public virtual string ContentFolderPath {
            get {
                // 親フォルダを取得
                var parentFolder = GetParent<ContentFolderWrapper>();
                if (parentFolder == null) {
                    return FolderName;
                }
                return $"{parentFolder.ContentFolderPath}/{FolderName}";
            }
        }
        // OS上の出力先フォルダのパス
        public virtual string ContentOutputFolderPath {
            get {
                string osFolderName;
                // ルートフォルダのIdを取得
                var rootFolder = Entity.GetRootFolder();
                // ContentFolderRootEntityを取得
                var rootFolderEntity = ContentFolderRootEntity.GetFolderRoot(rootFolder.Id);
                if (rootFolderEntity == null) {
                    throw new Exception("Root folder not found");
                }
                if (IsRootFolder) {
                    return rootFolderEntity.ContentOutputFolderPrefix;
                } else {
                    // FolderNameに:、\、/が含まれている場合は文字を削除
                    string modifiedFolderName = FolderName;
                    if (FolderName.Contains(':') || FolderName.Contains('\\') || FolderName.Contains('/')) {
                        modifiedFolderName = FolderName.Replace(":", "").Replace("\\", "").Replace("/", "");
                    }
                    osFolderName = $"{Parent?.ContentOutputFolderPath}{System.IO.Path.DirectorySeparatorChar}{modifiedFolderName}";
                }

                return osFolderName;
            }
        }

        // ルートフォルダか否か
        public bool IsRootFolder {
            get {
                // このオブジェクトのIdと一致するIdをContentFolderRootEntityが存在するか
                var rootId = Entity.GetRootFolder().Id;
                return rootId != null && rootId.Equals(Id);
            }
        }

        public const string VectorDBPropertiesName = "VectorDBProperties";
        public List<VectorDBProperty> ReferenceVectorSearchProperties {
            get {
                if (Entity.ExtendedProperties.TryGetValue(VectorDBPropertiesName, out var propList) && propList is List<Dictionary<string, object>>) {
                    List<VectorDBProperty> result = [];
                    foreach (var item in (List<Dictionary<string, object>>)propList) {
                        VectorDBProperty vectorDBProperty = new() {
                            FolderId = item["FolderId"]?.ToString() ?? Id,
                            VectorDBItemName = item["VectorDBItemName"]?.ToString() ?? VectorDBItem.GetDefaultVectorDB().Name,
                        };
                        result.Add(vectorDBProperty);
                    }
                    return result;
                }
                return [];
            }
            set {
                Entity.ExtendedProperties[FileSystemFolderPathName] = value;
            }

        }

        //　フォルダ名
        public virtual string FolderName {
            get {
                return Entity.FolderName;
            }
            set {
                Entity.FolderName = value;
            }
        }
        // Description
        public virtual string Description {
            get {
                return Entity.Description;
            }
            set {
                Entity.Description = value;
            }
        }


        // FileSystemFolderPath 名前
        public const string FileSystemFolderPathName = "FileSystemFolderPath";

        public string FileSystemFolderPath {
            get {
                if (Entity.ExtendedProperties.TryGetValue(FileSystemFolderPathName, out var path) && path is string) {
                    return (string)path;
                }
                return "";
            }
            set {
                Entity.ExtendedProperties[FileSystemFolderPathName] = value;
            }
        }

        // 削除
        public virtual void Delete() {
            // ベクトルを全削除
            VectorDBEmbedding.DeleteEmbeddingsByFolder(VectorDBPropertiesName, Id);
            using PythonAILibDBContext db = new();
            db.ContentFolders.Remove(Entity);
            db.SaveChanges();

        }


        public virtual List<T> GetItems<T>(bool isSync = true) where T : ContentItemWrapper {
            if (isSync) {
                // SyncItems
                SyncItems();
            }
            List<T> items = ContentItemWrapper.GetItems<T>(this);
            return items;
        }

        public virtual void SyncItems() { }

        public virtual List<T> GetChildren<T>() where T : ContentFolderWrapper {
            List<T> children = [];
            foreach (var child in Entity.GetChildren()) {
                T? childFolder = (T?)Activator.CreateInstance(typeof(T));
                if (childFolder != null) {
                    childFolder.Entity = child;
                    children.Add(childFolder);
                }
            }
            return children;
        }

        // 親フォルダ
        public virtual T? GetParent<T>() where T : ContentFolderWrapper {
            var folder = ContentFolderEntity.GetFolder(Entity.ParentId);
            if (folder == null) {
                return null;
            }
            T? result = (T?)Activator.CreateInstance(typeof(T));
            if (result == null) {
                throw new Exception("Failed to create instance of ContentFolderWrapper");
            }
            result.Entity = folder;

            return result;

        }

        // フォルダを移動する
        public void MoveTo(ContentFolderWrapper toFolder) {
            // 自分自身を移動
            if (toFolder.Id == Id) {
                return;
            }
            if (Parent == null) {
                return;
            }
            Entity.ParentId = toFolder.Id;
            Save();
        }

        // 名前を変更
        public void Rename(string newName) {
            FolderName = newName;
            Save();
        }

        // 保存
        public virtual void Save() {
            Entity.SaveExtendedPropertiesJson();

            using PythonAILibDBContext db = new();
            var folder = db.ContentFolders.FirstOrDefault(x => x.Id == Id);
            if (folder == null) {
                db.ContentFolders.Add(Entity);
            } else {
                db.ContentFolders.Entry(folder).CurrentValues.SetValues(Entity);
            }
            db.SaveChanges();

        }

        public virtual ContentFolderWrapper CreateChild(string folderName) {
            ContentFolderEntity childFolder = new() {
                ParentId = Id,
                FolderName = folderName,
                FolderTypeString = FolderTypeString,
            };
            return new ContentFolderWrapper() { Entity = childFolder };
        }

        // ステータス用の文字列
        public virtual string GetStatusText() {
            return "";
        }



        public virtual void AddItem(ContentItemWrapper item, bool applyGlobalAutoAction = false, Action<ContentItemWrapper>? afterUpdate = null) {
            // itemにFolderIdを設定
            item.Entity.FolderId = Id;

            if (applyGlobalAutoAction) {
                Task.Run(() => {
                    // Apply automatic processing
                    Task<ContentItemWrapper> updatedItemTask = AutoProcessRuleController.ApplyGlobalAutoActionAsync(item);
                    if (updatedItemTask.Result != null) {
                        item = updatedItemTask.Result;
                    }
                    item.Save();
                    afterUpdate?.Invoke(item);
                    // 通知
                    LogWrapper.Info(PythonAILibStringResources.Instance.AddedItems);
                });
            } else {
                // 保存
                item.Save();
                afterUpdate?.Invoke(item);
                // 通知
                LogWrapper.Info(PythonAILibStringResources.Instance.AddedItems);
            }
        }

        public VectorDBProperty GetMainVectorSearchProperty() {
            VectorDBProperty searchProperty = new() {
                FolderId = Id,
                TopK = 4,
                VectorDBItemName = VectorDBItem.GetDefaultVectorDB().Name,
            };
            return searchProperty;
        }

        public List<VectorDBProperty> GetVectorSearchProperties() {
            List<VectorDBProperty> searchProperties =
            [
                GetMainVectorSearchProperty(),
                // ReferenceVectorDBItemsに設定されたVectorDBItemを取得
                .. ReferenceVectorSearchProperties,
            ];
            return searchProperties;
        }
        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            ContentFolderWrapper other = (ContentFolderWrapper)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

        // ObjectIdからContentFolderWrapperを取得
        public static T? GetFolderById<T>(string? id) where T: ContentFolderWrapper{
            if (string.IsNullOrEmpty(id)) {
                return null;
            }

            var folder = ContentFolderEntity.GetFolder(id);
            if (folder == null) {
                return null;
            }
            T? result = (T?)Activator.CreateInstance(typeof(T));
            if (result == null) {
                throw new Exception("Failed to create instance of ContentFolderWrapper");
            }
            result.Entity = folder;
            return result;
        }

        // ToDict
        public Dictionary<string, object> ToDict() {
            // ExtendedPropertiesをJsonに変換
            Entity.SaveExtendedPropertiesJson();
            Dictionary<string, object> dict = new() {
                { "Id", Id },
                { "FolderName", FolderName },
                { "Description", Description },
                { "IsRootFolder", IsRootFolder },
                { "FolderTypeString", FolderTypeString },
                { "ExtendedPropertiesJson", Entity.ExtendedPropertiesJson },
            };
            if (Parent != null) {
                dict["Parent"] = Parent.ToDict();
            }

            return dict;
        }
        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<ContentFolderWrapper> items) {
            return items.Select(item => item.ToDict()).ToList();
        }

        // FromDict
        public static ContentFolderWrapper FromDict(Dictionary<string, object> dict) {
            ContentFolderEntity folderEntity = new() {
                Id = dict["Id"]?.ToString() ?? "",
                ParentId = dict["ParentId"]?.ToString() ?? null,
                FolderName = dict["FolderName"]?.ToString() ?? "",
                Description = dict["Description"]?.ToString() ?? "",
                FolderTypeString = dict["FolderTypeString"]?.ToString() ?? "",
                ExtendedPropertiesJson = dict["ExtendedPropertiesJson"].ToString() ?? "{}",
            };
            ContentFolderWrapper folder = new() { Entity = folderEntity };
            return folder;

        }

        // FromDictList
        public static List<ContentFolderWrapper> FromDictList(IEnumerable<Dictionary<string, object>> dicts) {
            return dicts.Select(dict => FromDict(dict)).ToList();
        }
    }
}
