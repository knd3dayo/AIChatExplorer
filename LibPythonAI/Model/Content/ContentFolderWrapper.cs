using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

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
            set {
                Entity.Id = value;
            }
        }
        public string ParentId {
            get {
                return Entity.ParentId ?? "";
            }
            set {
                Entity.ParentId = value;
            }
        }

        public bool IsRootFolder {
            get {
                return Entity.IsRootFolder;
            }
            set {
                Entity.IsRootFolder = value;
            }
        }

        public string ExtendedPropertiesJson {
            get {
                return Entity.ExtendedPropertiesJson;
            }
            set {
                Entity.ExtendedPropertiesJson = value;
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
        public virtual async Task<string> GetContentFolderPath() {
            var parentFolder = await GetParent<ContentFolderWrapper>();
            if (parentFolder == null) {
                return FolderName;
            }
            var parentFolderPatn = await parentFolder.GetContentFolderPath();
            return $"{parentFolderPatn}/{FolderName}";
        }
 

        public const string VectorDBPropertiesName = "VectorDBProperties";
        public ObservableCollection<VectorSearchItem> ReferenceVectorSearchProperties {
            get {
                ObservableCollection<VectorSearchItem> vectorDBProperties = [];

                if (Entity.ExtendedProperties.TryGetValue(VectorDBPropertiesName, out var propList)) {
                    if (propList is string json) {
                        // JsonをパースしてVectorSearchItemのリストを取得
                        vectorDBProperties = [.. VectorSearchItem.FromListJson(json)];
                    }
                }
                // Addイベント発生時の処理
                vectorDBProperties.CollectionChanged += (sender, e) => {
                    if (e.NewItems != null) {
                        // Entityを更新
                        Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(vectorDBProperties);
                        Entity.SaveExtendedPropertiesJson();
                    }
                };
                // Removeイベント発生時の処理
                vectorDBProperties.CollectionChanged += (sender, e) => {
                    if (e.OldItems != null) {
                        // Entityを更新
                        Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(vectorDBProperties);
                        Entity.SaveExtendedPropertiesJson();
                    }
                };
                // Clearイベント発生時の処理
                vectorDBProperties.CollectionChanged += (sender, e) => {
                    // Entityを更新
                    Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(vectorDBProperties);
                    Entity.SaveExtendedPropertiesJson();
                };

                return vectorDBProperties;
            }
            set {
                // Entityを更新
                Entity.ExtendedProperties[VectorDBPropertiesName] = VectorSearchItem.ToListJson(value);
                Entity.SaveExtendedPropertiesJson();
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
        public virtual async Task Delete() {
            // ベクトルを全削除
            var contentFolderPath = await GetContentFolderPath();
            VectorEmbeddingItem.DeleteEmbeddingsByFolder(VectorDBPropertiesName, contentFolderPath);
            // APIを呼び出して、ContentFolderを削除
            ContentFolderRequest request = new(Entity);
            await PythonExecutor.PythonAIFunctions.DeleteContentFoldersAsync([request]);
        }


        public async Task<List<T>> GetItems<T>(bool isSync = true) where T : ContentItemWrapper {
            if (isSync) {
                // SyncItems
                await SyncItems();
            }
            List<T> items = await ContentItemWrapper.GetItems<T>(this);
            return items;
        }

        public virtual async Task SyncItems() {
            await Task.Run(() => { });
        }

        public virtual async Task<List<T>> GetChildren<T>(bool isSync = true) where T : ContentFolderWrapper {
            if (isSync) {
                // SyncFolders
                await SyncFolders();
            }
            List<T> children = [];
            // APIを呼び出して、子フォルダを取得
            List<ContentFolderEntity> childFolders = await PythonExecutor.PythonAIFunctions.GetChildFoldersByIdAsync(Entity.Id);

            foreach (var child in childFolders) {
                T? childFolder = (T?)Activator.CreateInstance(typeof(T));
                if (childFolder != null) {
                    childFolder.Entity = child;
                    children.Add(childFolder);
                }
            }
            return children;
        }

        public virtual async Task SyncFolders() {
            await Task.CompletedTask;
        }

        // 親フォルダ
        public virtual async Task<T?> GetParent<T>() where T : ContentFolderWrapper {
            if (Entity.ParentId == null) {
                return null;
            }
            // APIを呼び出して、親フォルダを取得
            ContentFolderEntity? parentFolder = await PythonExecutor.PythonAIFunctions.GetParentFolderByIdAsync(Entity.ParentId);
            if (parentFolder == null) {
                return null;
            }
            T? result = (T?)Activator.CreateInstance(typeof(T)) ?? throw new Exception("Failed to create instance of ContentFolderWrapper");
            result.Entity = parentFolder;

            return result;

        }


        // フォルダを移動する
        public void MoveTo(ContentFolderWrapper toFolder) {
            // 自分自身を移動
            if (toFolder.Id == Id) {
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
            // APIを呼び出して、ContentFolderを更新
            ContentFolderRequest request = new(Entity);
            PythonExecutor.PythonAIFunctions.UpdateContentFoldersAsync([request]);

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
        public virtual async Task<string> GetStatusText() {
            await Task.CompletedTask;
            return "";
        }



        public virtual async Task AddItemAsync(ContentItemWrapper item, bool applyGlobalAutoAction = false, Action<ContentItemWrapper>? afterUpdate = null) {
            // itemにFolderIdを設定
            item.Entity.FolderId = Id;

            if (applyGlobalAutoAction) {
                // Apply automatic processing
                var updatedItem = await AutoProcessRuleController.ApplyGlobalAutoActionAsync(item);
                if (updatedItem != null) {
                    item = updatedItem;
                }
                await item.Save();
                afterUpdate?.Invoke(item);
                // 通知
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AddedItems);
            } else {
                // 保存
                await item.Save();
                afterUpdate?.Invoke(item);
                // 通知
                LogWrapper.Info(PythonAILibStringResourcesJa.Instance.AddedItems);
            }
        }

        public async Task<VectorSearchItem> GetMainVectorSearchItem() {
            var item = VectorDBItem.GetDefaultVectorDB();
            var contentFolderPath = await GetContentFolderPath();
            VectorSearchItem searchProperty = item.CreateVectorSearchItem(Id, contentFolderPath);
            return searchProperty;
        }

        public async Task<ObservableCollection<VectorSearchItem>> GetVectorSearchProperties() {
            var item = await GetMainVectorSearchItem();
            ObservableCollection<VectorSearchItem> searchProperties =
            [
                item,
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
        public static async Task<T?> GetFolderById<T>(string? id) where T : ContentFolderWrapper {
            if (string.IsNullOrEmpty(id)) {
                return null;
            }
            // APIを呼び出して、ContentFolderEntityを取得
            ContentFolderEntity? folder = await PythonExecutor.PythonAIFunctions.GetContentFolderByIdAsync(id);

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
  
    }
}
