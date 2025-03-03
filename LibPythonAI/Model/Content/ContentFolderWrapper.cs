using LibPythonAI.Data;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Resources;

namespace LibPythonAI.Model.Content {
    public class ContentFolderWrapper {

        public ContentFolderWrapper(ContentFolderEntity contentFolder) {
            Entity = contentFolder;
        }

        public ContentFolderWrapper(ContentFolderWrapper? parent, string folderName) {
            Entity = new ContentFolderEntity() {
                ParentId = parent?.Entity.Id,
                FolderName = folderName,
            };
        }

        public ContentFolderEntity Entity { get; set; }

        // Parent
        public ContentFolderWrapper? Parent {
            get {
                using PythonAILibDBContext db = new();
                var parentFolder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.ParentId);
                if (parentFolder == null) {
                    return null;
                }
                return new ContentFolderWrapper(parentFolder);
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
                var parentFolder = GetParent();
                if (parentFolder == null) {
                    return FolderName;
                }
                return $"{parentFolder.ContentFolderPath}/{FolderName}";
            }
        }
        // OS上のフォルダのパス
        public virtual string ContentOutputFolderPath {
            get {
                string osFolderName;
                // 親フォルダを取得
                var parentFolder = GetParent();
                if (parentFolder == null) {
                    osFolderName = Entity.ContentOutputFolderPrefix;
                } else {
                    // FolderNameに:、\、/が含まれている場合は文字を削除
                    string modifiedFolderName = FolderName;
                    if (FolderName.Contains(':') || FolderName.Contains('\\') || FolderName.Contains('/')) {
                        modifiedFolderName = FolderName.Replace(":", "").Replace("\\", "").Replace("/", "");
                    }

                    osFolderName = $"{parentFolder.ContentOutputFolderPath}{System.IO.Path.DirectorySeparatorChar}{modifiedFolderName}";
                }
                return osFolderName;
            }
        }

        // ルートフォルダか否か
        public bool IsRootFolder {
            get {
                return Entity.IsRootFolder;
            }
            set {
                Entity.IsRootFolder = value;
            }
        }

        public List<VectorDBProperty> ReferenceVectorSearchProperties {
            get {
                List<VectorDBProperty> result = [];
                var items = Entity.VectorDBProperties;
                foreach (var item in items) {
                    result.Add(new VectorDBProperty(item));
                }

                return result;
            }
            set {
                List<VectorDBPropertyEntity> result = [];

                foreach (var item in value) {
                    result.Add(item.Entity);
                }
                Entity.VectorDBProperties = result;
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
                if (Entity.ExtendedProperties.TryGetValue(FileSystemFolderPathName, out var path)) {
                    return (string)path;
                } else {
                    return "";
                }
            }
            set {
                Entity.ExtendedProperties[FileSystemFolderPathName] = value;
            }
        }



        // 削除
        public virtual void Delete() {
            // ベクトルを全削除
            GetMainVectorSearchProperty().DeleteVectorDBCollection();
            using PythonAILibDBContext db = new PythonAILibDBContext();
            db.ContentFolders.Remove(Entity);
            db.SaveChanges();

        }


        public virtual List<ContentItemWrapper> GetItems() {
            using PythonAILibDBContext db = new PythonAILibDBContext();
            IEnumerable<ContentItemEntity> items = db.ContentItems.Where(x => x.FolderId == Entity.Id);

            return items.Select(x => new ContentItemWrapper(x)).ToList();
        }

        public virtual List<ContentFolderWrapper> GetChildren() {
            using PythonAILibDBContext db = new PythonAILibDBContext();
            var children = db.ContentFolders.Where(x => x.ParentId == Entity.Id);
            return children.Select(x => new ContentFolderWrapper(x)).ToList();
        }

        // 親フォルダ
        public virtual ContentFolderWrapper? GetParent() {
            using PythonAILibDBContext db = new PythonAILibDBContext();
            var parentFolder = db.ContentFolders.Find(Entity.ParentId);
            if (parentFolder == null) {
                return null;
            }
            return new ContentFolderWrapper(parentFolder);
        }

        // フォルダを移動する
        public void MoveTo(ContentFolderWrapper toFolder) {
            // 自分自身を移動
            if (toFolder.Entity.Id == Entity.Id) {
                return;
            }
            if (Parent == null) {
                return;
            }
            Entity.ParentId = toFolder.Entity.Id;
            Save();
        }

        // 名前を変更
        public void Rename(string newName) {
            FolderName = newName;
            Save();
        }

        // 保存
        public void Save() {
            Entity.SaveExtendedPropertiesJson();

            using PythonAILibDBContext db = new();
            var folder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.Id);
            if (folder == null) {
                db.ContentFolders.Add(Entity);
            } else {
                db.ContentFolders.Entry(folder).CurrentValues.SetValues(Entity);
            }
            db.SaveChanges();
        }

        public virtual ContentFolderWrapper CreateChild(string folderName) {
            ContentFolderEntity childFolder = new() {
                ParentId = Entity.Id,
                FolderName = folderName,
            };
            return new ContentFolderWrapper(childFolder);
        }

        // ステータス用の文字列
        public virtual string GetStatusText() {
            return "";
        }


        #region 検索
        // ClipboardItemを検索する。
        public IEnumerable<ContentItemWrapper> SearchItems(SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<Entity>を作成
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


        public VectorDBProperty GetMainVectorSearchProperty() {

            VectorDBProperty searchProperty = new(VectorDBItem.GetDefaultVectorDB(), this);
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
            return Entity.Id == other.Entity.Id;
        }
        public override int GetHashCode() {
            return Entity.Id.GetHashCode();
        }

        // ObjectIdからContentFolderWrapperを取得
        public static ContentFolderWrapper? GetFolderById(string id) {

            using PythonAILibDBContext db = new();
            var folder = db.ContentFolders.Find(id);
            if (folder == null) {
                return null;
            }
            return new ContentFolderWrapper(folder);
        }

    }
}
