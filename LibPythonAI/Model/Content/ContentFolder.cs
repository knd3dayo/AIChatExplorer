using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.Content {
    public  class ContentFolder {

        // 親フォルダ
        public virtual void Save() {
            Save<ContentFolder, ContentItem>();
        }
        // 削除
        public virtual void Delete() {
            Delete<ContentFolder, ContentItem>();
        }

        // 親フォルダ
        public virtual ContentFolder? GetParent() {
            return GetParent<ContentFolder>();
        }

        public virtual ContentFolder CreateChild(string folderName) {
            ContentFolder child = new() {
                FolderName = folderName,
                ParentId = Id,
                FolderType = FolderType,
            };
            return child;
        }

        public virtual List<T> GetChildren<T>() where T: ContentFolder{

            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            IEnumerable<T> folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<T>().ToList();
        }

        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public virtual List<T> GetItems<T>() where T : ContentItem {
            List<T> _items = [];
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<T>();
            var items = collection.Find(x => x.CollectionId == Id);
            foreach (var item in items) {
                _items.Add(item);
            }
            return _items.Cast<T>().ToList();

        }


        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // フォルダの種類
        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

        // プロパティ
        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        public List<VectorDBProperty> ReferenceVectorSearchProperties { get; set; } = [];

        // 拡張プロパティ
        public Dictionary<string, object> ExtendedProperties { get; set; } = new();


        public VectorDBProperty GetMainVectorSearchProperty() {

            VectorDBProperty searchProperty = new(VectorDBItem.GetDefaultVectorDB(), Id);
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

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled { get; set; } = false;

        // フォルダの絶対パス ファイルシステム用
        public virtual string FolderPath {
            get {
                // 親フォルダを取得
                var parentFolder = GetParent();
                if (parentFolder == null) {
                    return FolderName;
                }
                return $"{parentFolder.FolderPath}/{FolderName}";
            }
        }
        // ステータス用の文字列
        public virtual string GetStatusText() {
            return "";
        }

        //　フォルダ名
        public virtual string FolderName { get; set; } = "";
        // Description
        public virtual string Description { get; set; } = "";

        // Idを指定してフォルダを取得
        public static T? GetFolderById<T>(ObjectId id) where T : ContentFolder {
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            return collection.FindById(id);
        }

        protected T? GetParent<T>() where T : ContentFolder {
            if (ParentId == ObjectId.Empty) {
                return null;
            }
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            return collection.FindById(ParentId);
        }

        // 保存
        protected void Save<T1, T2>() where T1 : ContentFolder where T2 : ContentItem {

            // ベクトルDBのコレクションを更新
            GetMainVectorSearchProperty().UpdateVectorDBCollection(Description);

            IDataFactory dataFactory = PythonAILibManager.Instance.DataFactory;
            dataFactory.GetFolderCollection<T1>().Upsert((T1)this);
            // ItemsのIsReferenceVectorDBItemsSyncedをFalseに設定
            foreach (var item in GetItems<T2>()) {
                item.IsReferenceVectorDBItemsSynced = false;
                item.Save(false);
            }
        }
        protected void Delete<T1, T2>() where T1 : ContentFolder where T2 : ContentItem {
            DeleteFolder<T1, T2>((T1)this);
        }

        // フォルダを削除
        protected void DeleteFolder<T1, T2>(T1 folder) where T1 : ContentFolder where T2 : ContentItem {
            // folderの子フォルダを再帰的に削除
            foreach (var child in folder.GetChildren<T1>()) {
                if (child != null) {
                    DeleteFolder<T1, T2>(child);
                }
            }
            // folderのアイテムを削除
            var items = PythonAILibManager.Instance.DataFactory.GetItemCollection<T2>().Find(x => x.CollectionId == folder.Id);
            foreach (var item in items) {
                item.Delete();
            }

            // ベクトルを全削除
            folder.GetMainVectorSearchProperty().DeleteVectorDBCollection();
            // folderを削除
            var folderCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T1>();
            folderCollection.Delete(folder.Id);
        }

        // フォルダを移動する
        public void MoveTo(ContentFolder toFolder) {
            // 自分自身を移動
            ParentId = toFolder.Id;
            Save();
        }
        // 名前を変更
        public void Rename(string newName) {
            FolderName = newName;
            Save();
        }

        #region ベクトル検索
        // ReferenceVectorDBItemsからVectorDBItemを削除
        public void RemoveVectorSearchProperty(VectorDBProperty vectorDBItem) {
            List<VectorDBProperty> existingItems = new(ReferenceVectorSearchProperties.Where(x => x.VectorDBItemId == vectorDBItem.VectorDBItemId && x.FolderId == vectorDBItem.FolderId));
            foreach (var item in existingItems) {
                ReferenceVectorSearchProperties.Remove(item);
            }
        }
        // ReferenceVectorDBItemsにVectorDBItemを追加
        public void AddVectorSearchProperty(VectorDBProperty vectorDBItem) {
            var existingItems = ReferenceVectorSearchProperties.FirstOrDefault(x => x.VectorDBItemId == vectorDBItem.VectorDBItemId);
            if (existingItems == null) {
                ReferenceVectorSearchProperties.Add(vectorDBItem);
            }
        }

        #endregion

        public virtual void AddItem(ContentItem item, bool applyGlobalAutoAction = false, Action<ContentItem>? afterUpdate = null) {
            // CollectionNameを設定
            item.CollectionId = Id;
            if (applyGlobalAutoAction) {
                Task.Run(() => {
                    // Apply automatic processing
                    Task<ContentItem> updatedItemTask = AutoProcessRuleController.ApplyGlobalAutoAction(item);
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

        #region 検索
        // ClipboardItemを検索する。
        public IEnumerable<ContentItem> SearchItems(SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ContentItem>を作成
            IEnumerable<ContentItem> result = [];
            // 検索条件が空の場合は、結果を返す
            if (searchCondition.IsEmpty()) {
                return result;
            }

            // folder内のアイテムを保持するコレクションを取得
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetItemCollection<ContentItem>();
            var clipboardItems = collection.Find(x => x.CollectionId == this.Id).OrderByDescending(x => x.UpdatedAt);
            // Filterの結果を結果に追加
            result = Filter(clipboardItems, searchCondition);

            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (searchCondition.IsIncludeSubFolder) {
                // 対象フォルダの子フォルダを取得
                var folderCollection = libManager.DataFactory.GetFolderCollection<ContentFolder>();
                var childFolders = folderCollection.Find(x => x.ParentId == this.Id).OrderBy(x => x.FolderName);
                foreach (var childFolder in childFolders) {
                    // サブフォルダのアイテムを検索
                    var subFolderResult = childFolder.SearchItems(searchCondition);
                    // Filterの結果を結果に追加
                    result = result.Concat(subFolderResult);
                }
            }
            return result;
        }

        public IEnumerable<ContentItem> Filter(IEnumerable<ContentItem> liteCollection, SearchCondition searchCondition) {
            if (searchCondition.IsEmpty()) {
                return liteCollection;
            }

            var results = liteCollection;
            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(searchCondition.Description) == false) {
                if (searchCondition.ExcludeDescription) {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description) == false);
                } else {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Content) == false) {
                if (searchCondition.ExcludeContent) {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content) == false);
                } else {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Tags) == false) {
                if (searchCondition.ExcludeTags) {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags) == false);
                } else {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationName) == false) {
                if (searchCondition.ExcludeSourceApplicationName) {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationTitle) == false) {
                if (searchCondition.ExcludeSourceApplicationTitle) {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle));
                }
            }
            if (searchCondition.EnableStartTime) {
                results = results.Where(x => x.CreatedAt > searchCondition.StartTime);
            }
            if (searchCondition.EnableEndTime) {
                results = results.Where(x => x.CreatedAt < searchCondition.EndTime);
            }
            results = results.OrderByDescending(x => x.UpdatedAt);

            return results;
        }

        #endregion
    }
}
