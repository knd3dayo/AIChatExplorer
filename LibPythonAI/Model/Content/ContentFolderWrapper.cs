using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.Content {
    public class ContentFolderWrapper {

        public ContentFolderWrapper(ContentFolder contentFolder) {
            ContentFolderInstance = contentFolder;
        }

        public ContentFolderWrapper(ContentFolderWrapper? parent, string folderName) {
            ContentFolderInstance = new ContentFolder() {
                ParentId = parent?.Id ?? LiteDB.ObjectId.Empty,
                FolderName = folderName,
            };
        }

        protected ContentFolder ContentFolderInstance { get; set; }

        public LiteDB.ObjectId Id {
            get {
                return ContentFolderInstance.Id;
            }
            set {
                ContentFolderInstance.Id = value;
            }
        }
        // フォルダの種類
        public FolderTypeEnum FolderType {
            get {
                return ContentFolderInstance.FolderType;
            }
            set {
                ContentFolderInstance.FolderType = value;
            }
        }
        // プロパティ
        // 親フォルダのID
        public LiteDB.ObjectId ParentId {
            get {
                return ContentFolderInstance.ParentId;
            }
            set {
                ContentFolderInstance.ParentId = value;
            }
        }
        // フォルダの絶対パス ファイルシステム用
        public virtual string FolderPath {
            get {
                return ContentFolderInstance.FolderPath;
            }
        }
        // ルートフォルダか否か
        public bool IsRootFolder {
            get {
                return ContentFolderInstance.IsRootFolder;
            }
            set {
                ContentFolderInstance.IsRootFolder = value;
            }
        }

        public List<VectorDBProperty> ReferenceVectorSearchProperties {
            get {
                return ContentFolderInstance.ReferenceVectorSearchProperties;
            }
            set {
                ContentFolderInstance.ReferenceVectorSearchProperties = value;
            }
        }

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled {
            get {
                return ContentFolderInstance.IsAutoProcessEnabled;
            }
            set {
                ContentFolderInstance.IsAutoProcessEnabled = value;
            }
        }

        //　フォルダ名
        public virtual string FolderName {
            get {
                return ContentFolderInstance.FolderName;
            }
            set {
                ContentFolderInstance.FolderName = value;
            }
        }
        // Description
        public virtual string Description {
            get {
                return ContentFolderInstance.Description;
            }
            set {
                ContentFolderInstance.Description = value;
            }
        }


        // 削除
        public virtual void Delete() {
            // ベクトルを全削除
            GetMainVectorSearchProperty().DeleteVectorDBCollection();
            ContentFolderInstance.Delete();
        }


        public virtual List<ContentItemWrapper> GetItems() {
            var items = ContentFolderInstance.GetItems<ContentItem>();
            return items.Select(x => new ContentItemWrapper(x)).ToList();
        }

        public virtual List<ContentFolderWrapper> GetChildren() {
            var children = ContentFolderInstance.GetChildren<ContentFolder>();
            return children.Select(x => new ContentFolderWrapper(x)).ToList();
        }

        // 親フォルダ
        public virtual ContentFolderWrapper? GetParent() {
            var parentFolder = ContentFolderInstance.GetParent();
            if (parentFolder == null) {
                return null;
            }
            return new ContentFolderWrapper(parentFolder);
        }

        // フォルダを移動する
        public void MoveTo(ContentFolderWrapper toFolder) {
            // 自分自身を移動
            ParentId = toFolder.Id;
            Save();
        }
        // 名前を変更
        public void Rename(string newName) {
            FolderName = newName;
            Save();
        }

        // 保存
        public void Save() {
            // ベクトルDBのコレクションを更新
            GetMainVectorSearchProperty().UpdateVectorDBCollection(ContentFolderInstance.Description);
            ContentFolderInstance.Save();
        }

        public virtual ContentFolderWrapper CreateChild(string folderName) {
            return new ContentFolderWrapper(ContentFolderInstance.CreateChild(folderName));
        }

        // ステータス用の文字列
        public virtual string GetStatusText() {
            return "";
        }

        public VectorDBProperty GetMainVectorSearchProperty() {

            VectorDBProperty searchProperty = new(VectorDBItem.GetDefaultVectorDB(), ContentFolderInstance.Id);
            return searchProperty;
        }

        public List<VectorDBProperty> GetVectorSearchProperties() {
            List<VectorDBProperty> searchProperties =
            [
                GetMainVectorSearchProperty(),
                // ReferenceVectorDBItemsに設定されたVectorDBItemを取得
                .. ContentFolderInstance.ReferenceVectorSearchProperties,
            ];
            return searchProperties;
        }

        #region 検索
        // ClipboardItemを検索する。
        public IEnumerable<ContentItemWrapper> SearchItems(SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ContentItemInstance>を作成
            IEnumerable<ContentItemWrapper> result = [];
            // 検索条件が空の場合は、結果を返す
            if (searchCondition.IsEmpty()) {
                return result;
            }

            // folder内のアイテムを保持するコレクションを取得
            var clipboardItems = GetItems();
            // Filterの結果を結果に追加
            result = Filter(clipboardItems, searchCondition);

            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (searchCondition.IsIncludeSubFolder) {
                // 対象フォルダの子フォルダを取得
                foreach (var childFolder in GetChildren()) {
                    // サブフォルダのアイテムを検索
                    var subFolderResult = childFolder.SearchItems(searchCondition);
                    // Filterの結果を結果に追加
                    result = result.Concat(subFolderResult);
                }
            }
            return result;
        }

        public virtual void AddItem(ContentItemWrapper item, bool applyGlobalAutoAction = false, Action<ContentItemWrapper>? afterUpdate = null) {
            // CollectionNameを設定
            item.CollectionId = Id;
            if (applyGlobalAutoAction) {
                Task.Run(() => {
                    // Apply automatic processing
                    Task<ContentItemWrapper> updatedItemTask = AutoProcessRuleController.ApplyGlobalAutoAction(item);
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


        public IEnumerable<ContentItemWrapper> Filter(IEnumerable<ContentItemWrapper> liteCollection, SearchCondition searchCondition) {
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


        // ObjectIdからContentFolderWrapperを取得
        public static ContentFolderWrapper? GetFolderById(LiteDB.ObjectId id) {
            var folder = ContentFolder.GetFolderById<ContentFolder>(id);
            if (folder == null) {
                return null;
            }
            return new ContentFolderWrapper(folder);
        }

    }
}
